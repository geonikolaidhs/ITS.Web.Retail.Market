package gr.net.its.common;

/*
 * Runned by onPostExecute(). 
 * Runs on the UI thread after doInBackground(Params...). The specified result is the value returned by
 * doInBackground(Params...). This method won't be invoked if the task was cancelled.
 */
public interface PostExecuteRunnable
{
    void Run(Object result);
}