package gr.net.its.common;

/*
 * Runned by onProgressUpdate(). 
 * Runs on the UI thread after updateProgress(Progress...) is invoked. The specified values are the values
 * passed to updateProgress(Progress...).
 */
public interface ProgressUpdateRunnable
{
    void Run(Integer... values);
}