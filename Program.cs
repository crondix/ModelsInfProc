using System;
using System.Collections;
using System.Collections.Generic;

class Operator
{
    private int _nowTask;
    private Queue<int> _queueCalls;

  public  Operator()
    {
        this._queueCalls= new Queue<int>();
    }

    public int QueueLength => this._queueCalls.Count;
    public int nowTask => _nowTask;
    public bool isQueueFull => this._queueCalls.Count >= 2;
    public bool tryAddNewTask(int taskCompletionTime)
    {
        if (this._queueCalls.Count<2)
        {
            if (this._nowTask==0)
            {
                this._nowTask= taskCompletionTime;
               
            }
            else
            {
                this._queueCalls.Enqueue(taskCompletionTime);
               
            }
            return true;
        }
        else
        {
            return false; 
        }
       
    }
    public void Tick(int tickTime)
    {
        if (this._nowTask != 0)
        {
            this._nowTask-= tickTime;
            if (this._nowTask<=0)
            {
                if (this._nowTask != 0)
                {
                    if (this._queueCalls.Count > 0)
                    {
                        this._nowTask = this._queueCalls.Dequeue()- this._nowTask;
                        if (this._nowTask < 0)
                        {
                            Tick(_nowTask);
                        }
                    }
                    else
                    {
                        this._nowTask = 0;
                    }
                }
                else
                {
                    if (this._queueCalls.Count > 0)
                    {
                        this._nowTask = this._queueCalls.Dequeue();
                    }
                }
               
            }
        }

    }


}
class Program
{
    static void Main()
    {
        const int callsIteration = 201;
        const int iteration = 1;

        int tendenied = 0;
        for (int g = 0; g < iteration; g++)
        {
            Operator[] operators = new Operator[]
        {
            new Operator (),
            new Operator (),
            new Operator (),
            new Operator (),
            new Operator ()
        };
          Random random = new Random();
       
        int timeToNewRequest () { return random.Next(2, 9); };
        int newTaskTime() { return random.Next(10, 51); };
        //operators[0].tryAddNewTask(newTaskTime());
        
        
            int denied = 0;
            for (int i = 1; i < callsIteration; i++)
            {
                int time = timeToNewRequest();
                Console.WriteLine("Tick time:" + time);
                for (int k = 0; k < operators.Length; k++)
                {
                    operators[k].Tick(time);
                    Console.WriteLine($"{k + 1} operator task now:{operators[k].nowTask} queue langth:{operators[k].QueueLength}");
                }
                try
                {
                    int minLoadId = getMinLoadId(operators);
                    int TaskTime = newTaskTime();
                    if (minLoadId >= 0 && operators[minLoadId].tryAddNewTask(TaskTime))
                    {

                        Console.WriteLine($"{i}  CALL WAS RECEIVED AND DISTRIBUTED FOR {minLoadId + 1} OPERATOR, TASK TIME:{TaskTime}".ToUpper());

                    }
                    else
                    {
                        Console.WriteLine($"{i} itaration denied");
                        denied++;
                    }


                    Console.WriteLine($"----------------------------------------");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            Console.WriteLine($"all denied calls:{denied}");
            tendenied += denied;
        }
        Console.WriteLine($"среднее за 1000:{tendenied/1000}");



        static int getMinLoadId(Operator[] operators)
        {
            // Первый свободный оператор
            for (int i = 0; i < operators.Length; i++)
            {
                if (operators[i].nowTask == 0)
                {
                    return i;
                }
            }

            // Если все операторы заняты, ищем оператор с минимальной загрузкой
            int minLoadId = -1;
            int minLoad = int.MaxValue;

            for (int i = 0; i < operators.Length; i++)
            {
                if (!operators[i].isQueueFull)
                {
                    int load = operators[i].QueueLength;
                    if (load < minLoad)
                    {
                        minLoad = load;
                        minLoadId = i;
                    }
                }
               
            }
            return minLoadId;
        }

    }
}