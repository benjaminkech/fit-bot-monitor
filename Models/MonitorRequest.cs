using System;

namespace FitBot.Models
{
    public class MonitorRequest
    {
        public string Id { get; set; }
        public string Phone { get; set; }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                MonitorRequest request = (MonitorRequest)obj;
                return (Id == request.Id) && (Phone == request.Phone);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}