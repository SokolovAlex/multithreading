using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        #region simple
        // Метод выполняемый в качестве задачи
        static void MyTask()
        {
            Console.WriteLine("MyTask() №{0} запущен", Task.CurrentId);

            for (int count = 0; count < 10; count++)
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                Console.WriteLine("В методе MyTask №{0} подсчет равен {1}", Task.CurrentId, count);
            }

            Console.WriteLine("MyTask() #{0} завершен", Task.CurrentId);
        }

        static void Main1()
        {
            Console.WriteLine("Основной поток запущен");



            //// Использование фабрики задач
            //TaskFactory tf = new TaskFactory();
            //Task t1 = tf.StartNew(MyTask);

            //// Использование фабрики задач через задачу
            //Task t2 = Task.Factory.StartNew(MyTask);

            // Использование конструктора Task
            Task task1 = new Task(MyTask);
            task1.Start();


            Task task2 = new Task(MyTask);

            // Запустить задачу
            task1.Start();

            Task.Delay(1500).Wait();

            task2.Start();

            // Приостановить выполнение метода Main() пока обе задачи не завершатся
            /* task1.Wait();
            task2.Wait();*/

            Task.WaitAll(task1, task2);

            Console.WriteLine("Основной поток завершен");
            Console.ReadLine();
        }
        #endregion

        #region lambda expression
        static void Main2()
        {
            Console.WriteLine("Основной поток запущен");

            // Используем лямбда-выражение
            Task task1 = Task.Factory.StartNew(() => {
                Console.WriteLine("MyTask() №{0} запущен", Task.CurrentId);

                for (int count = 0; count < 10; count++)
                {
                    Task.Delay(500).Wait();
                    Console.WriteLine("В методе MyTask №{0} подсчет равен {1}", Task.CurrentId, count);
                }

                Console.WriteLine("MyTask() #{0} завершен", Task.CurrentId);
            });

            task1.Wait();

            Console.WriteLine("Основной поток завершен");
            Console.ReadLine();
        }
        #endregion

        #region cancelling
        // Метод выполняемый в качестве задачи
        static void MyTask(Object ct)
        {
            CancellationToken cancelTok = (CancellationToken)ct;
            cancelTok.ThrowIfCancellationRequested();

            Console.WriteLine("MyTask() №{0} запущен", Task.CurrentId);

            for (int count = 0; count < 10; count++)
            {
                // Используем опрос
                if (!cancelTok.IsCancellationRequested)
                {
                    Task.Delay(500).Wait();
                    Console.WriteLine("В методе MyTask №{0} подсчет равен {1}", Task.CurrentId, count);
                }
            }

            Console.WriteLine("MyTask() #{0} завершен", Task.CurrentId);
        }

        static void Main3()
        {
            Console.WriteLine("Основной поток запущен 3");

            // Объект источника признаков отмены
            CancellationTokenSource cancelTokSSrc = new CancellationTokenSource();

            // Запустить задачу, передав ей признак отмены
            Task tsk = Task.Factory.StartNew(MyTask, cancelTokSSrc.Token, cancelTokSSrc.Token);

            Task.Delay(2000).Wait();
            try
            {
                Console.WriteLine("отменяем задачу");
                // отменить задачу
                cancelTokSSrc.Cancel();
                tsk.Wait();
            }
            catch (AggregateException exc)
            {
                if (tsk.IsCanceled)
                    Console.WriteLine("Задача tsk отменена");
            }
            catch (Exception exc)
            {
                if (tsk.IsCanceled)
                    Console.WriteLine("Задача tsk отменена");
            }
            finally
            {
                cancelTokSSrc.Dispose();
            }

            Console.WriteLine("Основной поток завершен");
            Console.ReadLine();
        }

        #endregion

        #region parallel

        // Методы, исполняемые как задача
        static void MyMeth()
        {
            Console.WriteLine("MyMeth запущен");
            for (int count = 0; count < 5; count++)
            {
                Task.Delay(500).Wait();
                Console.WriteLine("--> MyMeth Count=" + count);
            }
            Console.WriteLine("MyMeth завершен");
        }

        static void MyMeth2()
        {
            Console.WriteLine("MyMeth2 запущен");
            for (int count = 0; count < 5; count++)
            {
                Task.Delay(500).Wait();
                Console.WriteLine("--> MyMeth2 Count=" + count);
            }
            Console.WriteLine("MyMeth2 завершен");
        }

        static void Main()
        {
            Console.WriteLine("Основной поток запущен");

            // Выполнить параллельно оба именованных метода
            Parallel.Invoke(MyMeth, MyMeth2);

            Console.WriteLine("Основной поток завершен");
        }

        #endregion



    }
}