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

        //static List<Account> accounts = new List<Account>() {

            //new Account() {BagdeNo="7000357@gmail.com",Roles="Admin,Editor",Password="abcadmin" },
            //new Account() {BagdeNo="7000355",Roles="Editor",Password="xyzeditor" }
           // foreach(var us in user){new Account() {BagdeNo="7000357@gmail.com",Roles="Admin,Editor",Password="abcadmin" }};
        
         //};

        public static Account GetAccountDetails(Account account)
        {
            //var accc = db.Users.Where(u => u.BadgeNo.ToLower() == account.BagdeNo && u.Password == account.Password).FirstOrDefault();
            //if (accc != null)
            //{
            //    Account acc = new Account() { BagdeNo = accc.BadgeNo, Roles = accc.Roles, Password = accc.Password };
            //    return acc;
            //}
            var intBagde = System.Int32.Parse(account.BagdeNo);
            System.Diagnostics.Debug.WriteLine(intBagde);
            var accc = leardbUser.user_vw.Where(u => u.badge_no.ToLower() == intBagde.ToString().ToLower() && u.password == account.Password).FirstOrDefault();
            System.Diagnostics.Debug.WriteLine(accc);
            var passEn = passSecure.EncryptPassword(account.Password);
            var accc1 = leardbUser.user_vw.Where(u => u.badge_no.ToLower() == intBagde.ToString().ToLower() && u.Employee_Password == passEn).FirstOrDefault();
            System.Diagnostics.Debug.WriteLine(accc1);
            if (accc != null)
            {
                var users = learEmployees.Database.SqlQuery<Approver>("Select * from approvers").ToList<Approver>();
                var Roles = "Default";
               

                foreach (Approver app in users)
                {
                    //System.Diagnostics.Debug.WriteLine(app.approver);
                    //var tempBadge = "0" + accc.badge_no;
                    if (accc.badge_no.Equals(System.Int32.Parse(app.approver.ToString()).ToString().ToLower()))
                    {
                        Roles = "Approver";
                        break;
                    }
                }
                Account acc = new Account() { BagdeNo = account.BagdeNo, Roles = Roles, Password = accc.password };
                return acc;
            }else if(accc1 != null)
            {
                var users = learEmployees.Database.SqlQuery<Approver>("Select * from approvers").ToList<Approver>();
                var Roles = "Default";

               // var tempBadge = "0" + accc1.badge_no;
                foreach (Approver app in users)
                {
                    //System.Diagnostics.Debug.WriteLine(app.approver);
                    if (accc1.badge_no.Equals(System.Int32.Parse(app.approver.ToString()).ToString().ToLower()))
                    {
                        Roles = "Approver";
                        break;
                    }
                }
                Account acc = new Account() { BagdeNo = account.BagdeNo, Roles = Roles, Password = accc1.Employee_Password };
                return acc;
            }
            else
            {
                return null;
            }
                      
        }
    }
}