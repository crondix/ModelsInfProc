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
    public bool isQueueFull => this._queueCalls.Count < 2;
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
        Operator[] operators = new Operator[]
        {
            new Operator (),
            new Operator (),
            new Operator (),
            new Operator (),
            new Operator ()
        };
          Random random = new Random();
       
        int timeToNewRequest () { return random.Next(2, 4); };
        int newTaskTime() { return random.Next(40, 50); };
        //operators[0].tryAddNewTask(newTaskTime());
        int denied=0;
        for (int i=1; i<50; i++)
        {
            int time = timeToNewRequest();
            Console.WriteLine("Tick time:" + time);
            for (int k = 0; k < operators.Length; k++)
            {
                operators[k].Tick(time);
                Console.WriteLine($"{k+1} operator task now:{operators[k].nowTask} queue langth:{operators[k].QueueLength}") ;
            }
            try
            {
                int minLoadId = getMinLoadId(operators);
                int TaskTime = newTaskTime();
                if (minLoadId >= 0 && operators[minLoadId].tryAddNewTask(TaskTime)) {
                    
                    Console.WriteLine($"{i} call for {minLoadId + 1} operator, task time:{TaskTime}, task at queue:{operators[minLoadId].QueueLength}");

                }
                else
                {
                    Console.WriteLine($"{i} itaration denied");
                    denied++;
                }
               
               
                Console.WriteLine($"----------------------------------------");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
        Console.WriteLine($"all denied calls:{denied}");


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
                int load = operators[i].QueueLength + (operators[i].isQueueFull ? 1 : 0);
                if (load < minLoad)
                {
                    minLoad = load;
                    minLoadId = i;
                }
            }
            return minLoadId;
        }

    }
}