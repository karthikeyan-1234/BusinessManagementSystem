using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Auth
{
    public class PermissionAttribute: Attribute
    {
        public string permission_list { get; set; }

        public PermissionAttribute(string permission_list) { 
        
            this.permission_list = permission_list;
        }
    }
}
