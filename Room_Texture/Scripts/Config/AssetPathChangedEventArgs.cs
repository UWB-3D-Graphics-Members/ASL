using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWB_Texturing
{
    public delegate void AssetPathChangedEventHandler(AssetPathChangedEventArgs e);

    public class AssetPathChangedEventArgs : System.EventArgs
    {
        private string oldAbsoluteAssetRootFolder;
        private string newAbsoluteAssetRootFolder;

        public AssetPathChangedEventArgs(string oldAbsoluteAssetRootFolder, string newAbsoluteAssetRootFolder)
        {
            this.oldAbsoluteAssetRootFolder = oldAbsoluteAssetRootFolder ?? string.Empty;
            this.newAbsoluteAssetRootFolder = newAbsoluteAssetRootFolder ?? string.Empty;
        }

        public new AssetPathChangedEventArgs Empty
        {
            get
            {
                return new AssetPathChangedEventArgs(string.Empty, string.Empty);
            }
        }
        public string OldRootFolder
        {
            get
            {
                return oldAbsoluteAssetRootFolder;
            }
        }
        public string NewRootFolder
        {
            get
            {
                return newAbsoluteAssetRootFolder;
            }
        }
    }
}