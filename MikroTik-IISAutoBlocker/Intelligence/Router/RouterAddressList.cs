using MikroTik_IISAutoBlocker.Models;
using System;

namespace MikroTik_IISAutoBlocker.Intelligence.Router
{
  
    /// <summary>
    /// MiktoTik Firewall Router Address List Management
    /// </summary>
    internal class RouterAddressList : RouterBase
    {
        // https://help.mikrotik.com/docs/spaces/ROS/pages/130220135/Address-lists

        /// <summary>
        /// AddressList name for lookups and Add to
        /// </summary>
        private readonly string _listName;

        /// <summary>
        /// Subpath for Firewall AddressList API calls
        /// </summary>
        private const string _subpath = "ip/firewall/address-list";

        public RouterAddressList()
        {
            // Get List Name from Config
            this._listName = Properties.Settings.Default.RouterAddressListName;
        }


        /// <summary>
        /// Create Get URI for AddressList lookups based on the subpath and list name
        /// </summary>
        /// <returns>subpath with list Querystring</returns>
        private string BuildLookup()
        {
            return $"{_subpath}?list={_listName}";
        }



        /// <summary>
        /// Get Current List
        /// </summary>
        /// <returns>RAW JSON</returns>
        public string GetCurrentList()
        {
            var response = base.DoGet(BuildLookup());
            if (response.Item1)
            {
                return response.Item2;
            }
            return null;
        }



        /// <summary>
        /// Add Entry to List
        /// </summary>
        /// <param name="item">Item Details to log</param>
        /// <returns>true if added all okay</returns>
        public bool AddItem(RouterAddressItem item)
        {
            if (item != null && !string.IsNullOrWhiteSpace(item.Address))
            {
                return base.DoPut(_subpath, item.GetJson());
            }
            return false;
        }
    }
}
