using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using Windows.System.Threading;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams; // DataReader/DataWriter & Streams
using Windows.Security.Cryptography; // Convert string to bytes
using System.Runtime.InteropServices.WindowsRuntime; // Used for ToArray extension method for IBuffers (to convert them to byte arrays)
#endif

namespace UWBNetworkingPackage
{
    public class Socket_Base_Hololens : Socket_Base
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        public static void SendFileAsync(string filepath, StreamSocket socket)
        {
            SendFilesAsync(new string[1] { filepath }, socket);
        }

        public static async void SendFilesAsync(string[] filepaths, StreamSocket socket)
        {
            foreach (string filepath in filepaths)
            {
                //Debug.Log("Sending " + Path.GetFileName(filepath));
            }

            MemoryStream ms = new MemoryStream();
            PrepSocketData(filepaths, ref ms);
            DataWriter writer = new DataWriter(socket.OutputStream);
            writer.WriteBytes(ms.ToArray());
            await writer.StoreAsync();
            // uint numBytesSend = await writer.StoreAsync();

            socket.Dispose();
        }

        public static void PrepSocketData(string[] filepaths, ref MemoryStream ms)
        {
            string header = BuildSocketHeader(filepaths);
            
            byte[] headerData = StringToBytes(header);
            // Add header data length
            ms.Write(System.BitConverter.GetBytes(headerData.Length), 0, System.BitConverter.GetBytes(headerData.Length).Length);
            // Add header data
            ms.Write(headerData, 0, headerData.Length);

            foreach (string filepath in filepaths)
            {
                byte[] fileData = File.ReadAllBytes(filepath);
                // Add file data length
                ms.Write(System.BitConverter.GetBytes(fileData.Length), 0, System.BitConverter.GetBytes(fileData.Length).Length);
                // Add file data
                ms.Write(fileData, 0, fileData.Length);
            }
        }

        public static string BuildSocketHeader(string[] filepaths)
        {
            System.Text.StringBuilder headerBuilder = new System.Text.StringBuilder();

            foreach (string filepath in filepaths)
            {
                headerBuilder.Append(Path.GetFileName(filepath));
                headerBuilder.Append(';');
            }
            headerBuilder.Remove(headerBuilder.Length - 1, 1); // Remove the last separator (';')

            return headerBuilder.ToString();
        }

        public static async void ReceiveFilesAsync(StreamSocket socket, string receiveDirectory)
        {
            if (!Directory.Exists(receiveDirectory))
            {
                AbnormalDirectoryHandler.CreateDirectory(receiveDirectory);
            }

            MemoryStream fileStream = new MemoryStream();

            DataReader reader = new DataReader(socket.InputStream);

            uint bufferLength = 1048576;
            int numBytesReceived = 0;
            int headerIndex = 0;
            int dataLengthIndex = 0;
            string dataHeader = string.Empty;
            
            do
            {
                numBytesReceived = (int)(await reader.LoadAsync(bufferLength));
                int numBytesAvailable = numBytesReceived;
                int dataIndex = 0;

                // If there are any bytes that continue a file from the last buffer read, handle that here
                //if (numBytesRemaining > 0)
                if(dataLengthIndex > 0 && dataLengthIndex < numBytesReceived)
                {
                    byte[] bytesRemaining = reader.ReadBuffer((uint)numBytesAvailable).ToArray();
                    fileStream.Write(bytesRemaining, 0, numBytesAvailable);

                    string filename = dataHeader.Split(';')[headerIndex++];
                    File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());

                    // fileStream.Close(); // Doesn't exist in UWP?
                    fileStream.Dispose();
                    fileStream = new MemoryStream();
                }

                //while (reader.UnconsumedBufferLength > 0)
                while (dataLengthIndex >= 0 && dataLengthIndex < numBytesReceived)
                {
                    // Read in the first four bytes for the length
                    int dataLength = 0;
                    if (dataLengthIndex <= numBytesReceived - 4)
                    {
                        // Convert to length
                        byte[] dataLengthBuffer = reader.ReadBuffer(4).ToArray();
                        dataLength = System.BitConverter.ToInt32(dataLengthBuffer, 0);

                        if (dataLength <= 0)
                        {
                            // Handle case where end of stream is reached
                            break;
                        }
                    }
                    //else
                    else if (dataLengthIndex < numBytesReceived && dataLengthIndex > numBytesReceived - 4)
                    {
                        // Else length bytes are split between reads...
                        MemoryStream dataLengthMS = new MemoryStream();
                        uint numDataLengthBytes = (uint)(numBytesReceived - dataLengthIndex);
                        dataLengthMS.Write(reader.ReadBuffer(numDataLengthBytes).ToArray(), dataLengthIndex, (int)numDataLengthBytes);


                        numBytesReceived = (int)(await reader.LoadAsync(bufferLength));
                        numBytesAvailable = numBytesReceived;
                        numDataLengthBytes = 4 - numDataLengthBytes;
                        dataLengthMS.Write(reader.ReadBuffer(numDataLengthBytes).ToArray(), 0, (int)numDataLengthBytes);

                        dataLength = System.BitConverter.ToInt32(dataLengthMS.ToArray(), 0);
                        dataLengthIndex -= numBytesReceived;
                        dataLengthMS.Dispose();
                    }

                    // Handle instances where whole file is contained in part of buffer
                    if (numBytesAvailable > 0 && dataIndex + dataLength < numBytesAvailable)
                    {
                        if (dataHeader.Equals(string.Empty))
                        {
                            fileStream.Write(reader.ReadBuffer((uint)dataLength).ToArray(), 0, dataLength);

                            dataHeader = BytesToString(fileStream.ToArray());

                            fileStream.Dispose();
                            fileStream = new MemoryStream();
                        }
                        else
                        {
                            fileStream.Write(reader.ReadBuffer((uint)dataLength).ToArray(), 0, dataLength);

                            string filename = dataHeader.Split(';')[headerIndex++];
                            File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());

                            fileStream.Dispose();
                            fileStream = new MemoryStream();
                        }
                    }
                }

                // Write remainder of bytes in buffer to the file memory stream to store for the next buffer read
                if (numBytesAvailable < 0)
                {
                    break;
                }
                else
                {
                    fileStream.Write(reader.ReadBuffer((uint)numBytesAvailable).ToArray(), 0, numBytesAvailable);
                    dataLengthIndex -= (int)numBytesReceived;
                }
            } while (numBytesReceived > 0);

            fileStream.Dispose();
        }

        public static string BytesToString(byte[] bytes)
        {
            //CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, reader.ReadBuffer(numBytes));
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static byte[] StringToBytes(string str)
        {
            //CryptographicBuffer.ConvertStringToBinary(header, BinaryStringEncoding.Utf8).ToArray();
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
#endif

    }
}