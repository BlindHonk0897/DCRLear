using DCRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.DA
{
    public static class Repository
    {
       public static lear_DailiesCertificationRequirementEntities db = new lear_DailiesCertificationRequirementEntities();
       public static gatepassEntities leardbUser = new gatepassEntities();
       public static commonEmployeesEntities learEmployees = new commonEmployeesEntities();
       public static PasswordSecurity passSecure = new PasswordSecurity();
       static List<User> user = db.Users.ToList();

        public static Account GetAccountDetails(Account account)
        {        
            var intBagde = System.Int32.Parse(account.BagdeNo);
            System.Diagnostics.Debug.WriteLine(intBagde); //Console Display For Debug Purposes

            // get User from user_vw with Default Password (Lear)
            //var accc = leardbUser.user_vw.Where(u => u.badge_no.ToLower() == intBagde.ToString().ToLower() && u.password == account.Password).FirstOrDefault();
            //System.Diagnostics.Debug.WriteLine(accc); //Console Display For Debug Purposes

            // get User from users_vw // Comment next Line For Deploying // Uncomment for Testing--
            var accc = leardbUser.users_vw.Where(u => u.Employee_ID.ToLower() == account.BagdeNo.ToString().ToLower() && "Lear" == account.Password).FirstOrDefault();

            // encrypt Password
            var passEn = passSecure.EncryptPassword(account.Password);

            // get User from user_vw with their Own Password:
            //var accc1 = leardbUser.user_vw.Where(u => u.badge_no.ToLower() == intBagde.ToString().ToLower() && u.Employee_Password == passEn).FirstOrDefault();

            // get User from users_vw 
            var accc1 = leardbUser.users_vw.Where(u => u.Employee_ID.ToLower() == account.BagdeNo.ToString().ToLower() && u.Employee_Password == passEn).FirstOrDefault();

            System.Diagnostics.Debug.WriteLine(accc1);//Console Display For Debug Purposes

            // Check if User with Default Password is exist
            if (accc != null)
            {
                // if exist---

                // Get all approvers from Database
                var users = learEmployees.Database.SqlQuery<Approver>("Select * from approvers").ToList<Approver>();

                // Set variable Roles as 'Default'
                var Roles = "Default";
               
                // Check if Default User is an Approver VIA foreach loop
                foreach (Approver app in users)
                {
                    
                    if (accc.Employee_ID.Equals(app.approver.ToString().ToLower()))
                    {
                        // if User is consider as Approver set variable Roles to 'Approver'
                        Roles = "Approver";
                        break;
                    }
                }

                // Initialize account and set its attributes by the Defaut User
                Account acc = new Account() { BagdeNo = account.BagdeNo, Roles = Roles, Password = accc.Employee_Password };
                return acc;
            }else if(accc1 != null) // else if Default User not exist check User with its prefer password 
            {
                // if exist---

                // Get all approvers from Database
                var users = learEmployees.Database.SqlQuery<Approver>("Select * from approvers").ToList<Approver>();

                // Set variable Roles as 'Default'
                var Roles = "Default";

                // Check if Default User is an Approver VIA foreach loop
                foreach (Approver app in users)
                {               
                    if (accc1.Employee_ID.Equals(app.approver.ToString().ToLower()))
                    {
                        // if User is consider as Approver set variable Roles to 'Approver'
                        Roles = "Approver";
                        break;
                    }
                }
                // Initialize account and set its attributes by the Defaut User
                Account acc = new Account() { BagdeNo = account.BagdeNo, Roles = Roles, Password = accc1.Employee_Password };
                return acc;
            }
            else // else just return null
            {
                return null;
            }
                      
        }
    }
}