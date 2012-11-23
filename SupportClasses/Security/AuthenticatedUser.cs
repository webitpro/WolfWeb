using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebIT.Temp.Models;
using WebIT.Lib;

namespace WebIT.Temp
{
    public class AuthenticatedUser
    {
        public enum Info
        {
            ID,
            Name,
            Roles
        }
        //PROPERTIES
        public int ID { get; set; }
        public string Name { get; set; }
        public SecurityRole[] Roles { get; set; }

        #region constructors
        private AuthenticatedUser()
        {
            Roles = new SecurityRole[0];
        }

        private AuthenticatedUser(int personID, string name)
        {
            ID = personID;
            Name = name;
            UpdateRolesFromDB();
        }
        #endregion

        //METHODS
        public override string ToString()
        {
            return String.Format("{0}\n{1}", ID, Name);
        }

        public static AuthenticatedUser FromString(string data)
        {
            string[] arr = data.Split('\n');
            AuthenticatedUser user = new AuthenticatedUser();
            user.ID = Convert.ToInt32(arr[0]);
            user.Name = arr[1];

            user.UpdateRolesFromDB();

            return user;
        }

        public static AuthenticatedUser FindUser(string email, string hashedPassword)
        {
            DBDataContext db = Utils.DB.GetContext();
            Account acc = db.Accounts.SingleOrDefault(x => x.Email.Equals(email) && x.Password == hashedPassword);
            if (acc != null)
            {
                return new AuthenticatedUser(acc.ID, String.Format("{0} {1}", acc.FirstName, acc.LastName));
            }
            return null;

        }

        private void UpdateRolesFromDB()
        {
            DBDataContext db = Utils.DB.GetContext();
            List<SecurityRole> secRoles = new List<SecurityRole>();
            foreach (var x in db.Roles.Where(x => x.Account.ID == this.ID).Select(x => x))
            {
                try
                {
                    secRoles.Add((SecurityRole)x.SecurityRole);
                }
                catch
                {
                }
            }
            this.Roles = secRoles.ToArray();
        }


    }
}