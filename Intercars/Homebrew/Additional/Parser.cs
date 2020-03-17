using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Homebrew
{
    public class Parser
    {
        #region Управление интерфейсом
        /// <summary>
        /// Доступ к окну лога
        /// </summary>
        protected RichTextBox DebugBox => Controls.DebugBox;
        /// <summary>
        /// Доступ к прогресс бару + авто-установка его числового отображения
        /// </summary>
        /// <summary>
        /// Доступ к сохранённым куки
        /// </summary>        
        protected void SetProgress(double value)
        {
            if (value >= 0 && value <= 100)
            {
                Controls.WorkProgressLabel.Set(value + "%");
            }
            Controls.WorkProgress.SetValue(value);
        }
        #endregion
        protected CookieContainer SavedCookies = new CookieContainer();
        #region Управление потоками
        private int threadCount = 0;
        protected delegate void ExecutionMethod(dynamic param);
        protected ExecutionMethod MethodToExecute;
        protected Stack<dynamic> Parametres = new Stack<object>();
        protected delegate void ExecuteHandler();
        protected event ExecuteHandler OnExecuteCompleted;
        protected void Execute(int threadCount)
        {
            for (int i = 0; threadCount > i; i++)
            {
                threadCount++;
                StartThread();
            }
        }
        /// <summary>
        /// Запускаем поток
        /// </summary>
        private void StartThread()
        {
            Thread thread = new Thread(() =>
            {
                while (Parametres.Count > 0)
                {
                    MethodToExecute(Parametres.Pop());
                }
                threadCount--;
                if (threadCount == 0)
                {
                    OnExecuteCompleted.Invoke();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
        #endregion
    }
}
