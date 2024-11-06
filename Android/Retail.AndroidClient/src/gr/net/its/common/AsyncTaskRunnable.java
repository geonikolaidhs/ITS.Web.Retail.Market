package gr.net.its.common;

/*
 * Runned by doInBackground(Params...).
 * Override this method to perform a computation on a background thread. The specified parameters are the parameters passed to execute(Params...) by the caller of this task. 
 * This method can call publishProgress(Progress...) to publish updates on the UI thread.
 */
public interface AsyncTaskRunnable
{
    Object Run(Object... params);
}