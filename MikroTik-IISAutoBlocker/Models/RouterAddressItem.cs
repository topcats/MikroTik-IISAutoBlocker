using System;

namespace MikroTik_IISAutoBlocker.Models
{
    internal class RouterAddressItem
    {

        /// <summary>
        /// ListName - Should be the group name
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// Single IP address or subnet range to be added to the list
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Optional comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Time /date when creating entry
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Optional Timeout in hours
        /// </summary>
        public int Timeout { get; set; }

    }
}
