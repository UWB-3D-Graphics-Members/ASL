using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWB_Texturing
{
    public delegate void AssetSubFolderChangedEventHandler(AssetSubFolderChangedEventArgs e);

    public class AssetSubFolderChangedEventArgs : System.EventArgs
    {
        private string oldAssetSubFolder;
        private string newAssetSubFolder;

        public AssetSubFolderChangedEventArgs(string oldAssetSubFolder, string newAssetSubFolder)
        {
            this.oldAssetSubFolder = oldAssetSubFolder ?? string.Empty;
            this.newAssetSubFolder = newAssetSubFolder ?? string.Empty;
        }

        public new AssetSubFolderChangedEventArgs Empty
        {
            get
            {
                return new AssetSubFolderChangedEventArgs(string.Empty, string.Empty);
            }
        }
        public string OldSubFolder
        {
            get
            {
                return oldAssetSubFolder;
            }
        }
        public string NewSubFolder
        {
            get
            {
                return newAssetSubFolder;
            }
        }
    }
}