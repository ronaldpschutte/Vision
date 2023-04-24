using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Helpers
{
    public static class TaskStatus
    {
        public static Dictionary<int, string> Status
        {
            get
            {
                Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
                //keyValuePairs.Add(1, "New");
                //keyValuePairs.Add(8, "In behandeling");
                //keyValuePairs.Add(30, "Wordt gepland");
                //keyValuePairs.Add(18, "Akkoord door klant");
                //keyValuePairs.Add(10, "Terugbellen door Synlogic");
                //keyValuePairs.Add(26, "Terugbellen door Klant");
                //keyValuePairs.Add(27, "Geüpdatet via e-mail");
                //keyValuePairs.Add(19, "Notitie van klant");
                //keyValuePairs.Add(7, "Waiting Customer");
                //keyValuePairs.Add(5, "Complete");
                //keyValuePairs.Add(11, "Escalatie");
                //keyValuePairs.Add(23, "Afspraak ingepland");
                //keyValuePairs.Add(17, "In de wacht");
                //keyValuePairs.Add(14, "Gefactureerd");
                //keyValuePairs.Add(13, "Wachten op akkoord");
                //keyValuePairs.Add(12, "Wachten op leverancier");
                //keyValuePairs.Add(24, "Wachten op collega");
                //keyValuePairs.Add(9, "Wachten op materiaal");
                //keyValuePairs.Add(28, "Materiaal is binnen");
                //keyValuePairs.Add(29, "Materiaal is klaar");
                ////keyValuePairs.Add(25, "Werk voor ServiceDesk");
                //keyValuePairs.Add(15, "Order aanpassen");
                //keyValuePairs.Add(16, "Inactief");
                keyValuePairs.Add(1, "New");
                keyValuePairs.Add(8, "Work in Progress");
                keyValuePairs.Add(30, "Is being planned");
                keyValuePairs.Add(18, "Agreement by customer");
                keyValuePairs.Add(10, "Call back by Synlogic");
                keyValuePairs.Add(26, "Call back by customer");
                keyValuePairs.Add(27, "Updated by email");
                keyValuePairs.Add(19, "Note from customer");
                keyValuePairs.Add(7, "Waiting Customer");
                keyValuePairs.Add(5, "Complete");
                keyValuePairs.Add(11, "Escalation");
                keyValuePairs.Add(23, "Appointment scheduled");
                keyValuePairs.Add(17, "On hold");
                keyValuePairs.Add(14, "Billed");
                keyValuePairs.Add(13, "Waiting for agreement");
                keyValuePairs.Add(12, "Waiting for supplier");
                keyValuePairs.Add(24, "Waiting for colleague");
                keyValuePairs.Add(9, "Waiting for material");
                keyValuePairs.Add(28, "Material is in");
                keyValuePairs.Add(29, "Material is ready");
                keyValuePairs.Add(15, "Adjust order");
                keyValuePairs.Add(16, "Inactive");
                return keyValuePairs;
            }
        }
        public static string GetStatus(int statusId)
        {
            if (Status.ContainsKey(statusId))
            {
                return Status[statusId];
            }
            throw new Exception($"Task StatusId {statusId} does not exist");

        }
        public static int GetStatus(string status)
        {
            if (Status.ContainsValue(status))
            {
                return Status.SingleOrDefault(x => x.Value == status).Key;
            }
            throw new Exception($"Task Status Value \"{status}\" does not exist");
        }
        
    }
}
