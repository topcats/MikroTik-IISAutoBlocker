using System;

namespace MikroTik_IISAutoBlocker.Models
{
    /// <summary>
    /// MikroTik Router Firewall Address Item
    /// </summary>
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


        /// <summary>
        /// Create JSON of Data
        /// </summary>
        public string GetJson()
        {
            string ipSubnet = this.Address;
            ipSubnet = ipSubnet.Substring(0, ipSubnet.LastIndexOf("."));
            ipSubnet = ipSubnet + ".0/24";

            string output = $"{{\"address\":\"{ipSubnet}\",\"disabled\":\"false\",\"list\":\"{this.ListName}\"";

            // Add Timeout
            if (this.Timeout > 1)
            {
                var timeOuttext = (this.Timeout >= 24) ? $"{(int)this.Timeout / 24}d {(int)this.Timeout % 24}:00:00" : $"{this.Timeout}:00:00";
                output += $",\"dynamic\":\"true\",\"timeout\":\"{timeOuttext}\"";
            }
            else
            {
                output += $",\"dynamic\":\"false\"";
            }

            // Add Comment
            if (!string.IsNullOrEmpty(this.Comment))
            {
                output += $",\"comment\":\"{this.Comment}\"";
            }

            return output + "}}";
        }
    }
}
