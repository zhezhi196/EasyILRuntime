using System;

namespace Module
{
    public class AsyncTask
    {
        public  Awaiter awaiter;

        public Awaiter GetAwaiter()
        {
            awaiter = new Awaiter();
            return awaiter;
        }
        
        public void SetResult()
        {
            awaiter.SetResult();
        }
    }
}