using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetect
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //处理未捕捉的异常
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                //处理UI执行处的异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(OnApplication_ThreadException);

                //处理非UI执行处的异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnCurrentDomain_UnhandledException);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                DH.Tools.IO.IOTools.WriteErrorLog("错误->", e.ToString());
            }            
        }

        #region 异常处理
        //处理UI执行处的异常
        static void OnApplication_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                string msg = string.Empty;
                Exception err = e.Exception as Exception;
                if (err != null)
                {
                    msg = string.Format("处理UI执行处异常Application_ThreadException:\n 异常类型:{0}\n异常信息:{1}\n异常位置:{2}\n",
                                         err.GetType().Name, err.Message, err.StackTrace);
                }
                else
                {
                    msg = string.Format("处理UI执行处错误Application_ThreadException:{0}\n", e);
                }
            }
            catch (Exception ex)
            {
            }
        }

        //处理非UI执行处的异常
        static void OnCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                string msg = string.Empty;
                Exception err = e.ExceptionObject as Exception;
                if (err != null)
                {
                    msg = string.Format("非UI执行处的异常CurrentDomain_UnhandledException:\n 异常类型:{0}\n异常信息:{1}\n异常位置:{2}\n",
                                         err.GetType().Name, err.Message, err.StackTrace);
                }
                else
                {
                    msg = string.Format("非UI执行处的异常CurrentDomain_UnhandledException:{0}\n", e);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(10 * 1000);
                }
            }
        }
        #endregion
    }
}
