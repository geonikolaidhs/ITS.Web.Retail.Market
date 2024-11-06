package gr.net.its.common;

import android.app.Dialog;
import android.os.AsyncTask;

/*
 * A General implementation of AsyncTask. Use setAsyncTaskRunner(AsyncTaskRunner) to define the code to run in background.
 * You can optionally set preExecute, postExecute, progressUpdate and a dialog to show on the UI thread.  
 */
public class GeneralAsyncTask extends AsyncTask<Object, Integer, Object>
{
    private Dialog dialog;
    private AsyncTaskRunnable asyncTaskRunner;
    private PreExecuteRunnable preExecuteRunner;
    private PostExecuteRunnable postExecuteRunner;
    private ProgressUpdateRunnable progressUpdateRunner;

    public void updateProgress(Integer... values)
    {
	publishProgress(values);
    }

    @Override
    protected void onPostExecute(Object result)
    {
	
	if (postExecuteRunner != null)
	{
	    postExecuteRunner.Run(result);
	}
	if(dialog != null && dialog.isShowing())
	{
	    dialog.dismiss();
	}
	
	super.onPostExecute(result);
	
    }

    @Override
    protected void onPreExecute()
    {	
	if(dialog !=null && !dialog.isShowing())
	{
	    dialog.show();
	}
	if (preExecuteRunner != null)
	{
	    preExecuteRunner.Run();
	}
	super.onPreExecute();
    }

    @Override
    protected void onProgressUpdate(Integer... values)
    {
	super.onProgressUpdate(values);
	if (progressUpdateRunner != null)
	{
	    progressUpdateRunner.Run(values);
	}
    }

    @Override
    protected Object doInBackground(Object... params)
    {
	if (asyncTaskRunner != null)
	{
	    return asyncTaskRunner.Run(params);
	}
	else
	{
	    return null;
	}
    }

    public Dialog getDialog()
    {
	return dialog;
    }

    public void setDialog(Dialog dialog)
    {
	this.dialog = dialog;
    }

    public AsyncTaskRunnable getRunner()
    {
	return asyncTaskRunner;
    }

    public void setRunner(AsyncTaskRunnable runner)
    {
	this.asyncTaskRunner = runner;
    }

    public PreExecuteRunnable getPreExecuteRunner()
    {
	return preExecuteRunner;
    }

    public void setPreExecuteRunner(PreExecuteRunnable preExecuteRunner)
    {
	this.preExecuteRunner = preExecuteRunner;
    }

    public PostExecuteRunnable getPostExecuteRunner()
    {
	return postExecuteRunner;
    }

    public void setPostExecuteRunner(PostExecuteRunnable postExecuteRunner)
    {
	this.postExecuteRunner = postExecuteRunner;
    }

    public ProgressUpdateRunnable getProgressUpdateRunner()
    {
	return progressUpdateRunner;
    }

    public void setProgressUpdateRunner(ProgressUpdateRunnable progressUpdateRunner)
    {
	this.progressUpdateRunner = progressUpdateRunner;
    }

}
