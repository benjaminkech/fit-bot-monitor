using System;

namespace FitBot.Models
{
    public class MonitorRequest
    {
        public string CourseId { get; set; }
        public string UserId { get; set; }

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
                return (CourseId == request.CourseId) && (UserId == request.UserId);
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