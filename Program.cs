// Класс, представляющий оператора
class Operator
{
    // Текущая задача оператора
    private int _currentTask;
    // Очередь задач оператора
    private Queue<int> _taskQueue;
    // Общее время работы оператора
    private int _totalWorkTime;
    // Общее время простоя оператора
    private int _totalIdleTime;
    // Флаг, указывающий, работал ли оператор в предыдущем тике
    private bool _wasWorking;
    // Максимальная длина очереди оператора
    private int _maxQueueLength;
    // колличество выполненых задач
    private int _taskСompleted;


    // Конструктор класса оператора
    public Operator(int maxQueueLength)
    {
        this._taskQueue = new Queue<int>();
        this._totalWorkTime = 0;
        this._maxQueueLength = maxQueueLength;

    }

    public IEnumerable<int> TaskQueue => _taskQueue;
    // Свойство, возвращающее длину очереди задач
    public int QueueLength => this._taskQueue.Count;
    // Kолличество выполненых задач
    public int TaskСompleted => this._taskСompleted;
    // Текущая задача оператора
    public int CurrentTask => _currentTask;
    // Общее время работы оператора
    public int TotalWorkTime => _totalWorkTime;
    // Флаг, указывающий, заполнена ли очередь задач
    public bool IsQueueFull => this._taskQueue.Count >= 2;
    // Общее время простоя оператора
    public int TotalIdleTime => _totalIdleTime;
    // Максимальная длина очереди оператора
    public int MaxQueueLength => _maxQueueLength;

    // Попытка добавить новую задачу в очередь
    public bool TryAddNewTask(int taskCompletionTime)
    {
        if (this._taskQueue.Count < 2)
        {
            if (this._currentTask == 0)
            {
                this._currentTask = taskCompletionTime;
            }
            else
            {
                this._taskQueue.Enqueue(taskCompletionTime);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    // Метод, эмулирующий прошествие времени в системе операторов
    public void Tick(int tickTime)
    {
        if (_currentTask != 0)
        {
            _totalWorkTime += tickTime;
            
            _currentTask -= tickTime;

            if (_currentTask <= 0)
            {
                if (_taskQueue.Count > 0)
                {
                    if (_currentTask < 0) _currentTask = _taskQueue.Dequeue() - _currentTask;
                    else _currentTask = _currentTask = _taskQueue.Dequeue();
                    _taskСompleted++;
                }
                else
                {
                    _currentTask = 0;
                    _wasWorking = false;
                }
            }
            else
            {
                _wasWorking = true;
            }
        }
        else if (_wasWorking == false)
        {
            _totalIdleTime += tickTime;
        }
    }

    // Коэффициент загрузки оператора
    public double LoadCoefficient
    {
        get
        {
            double totalWorkTime = TotalWorkTime;
            if (_taskQueue.Count != 0)
            {
                foreach (var task in _taskQueue)
                {
                    totalWorkTime += task;
                }
            }
            if(_currentTask>0) totalWorkTime += _currentTask;
            double totalIdleTime = TotalIdleTime;
            double totalTime = totalWorkTime + totalIdleTime;

            if (totalTime == 0)
                return 0;

            return totalWorkTime / totalTime;
        }
    }
}

// Главный класс программы
class Program
{
    static void Main()
    {
        // Количество итераций комплекса
        const int complexIteration =1;
        const int callsIteration = 200;
        // Количество операторов
        const int numberOfOperators = 5;
        // Максимальная длина очереди оператора
        const int maxQueueLength = 2;
        // Минимальное время до нового запроса
        const int minTimeToNewRequest = 2;
        // Максимальное время до нового запроса
        const int maxTimeToNewRequest = 9;
        // Минимальное время задачи
        const int minTaskTime = 10;
        // Максимальное время задачи
        const int maxTaskTime = 51;


        // Количество отклонённых звонков
        int complexDenied = 0;

        for (int g = 0; g < complexIteration; g++)
        {
            // Массив операторов
            Operator[] operators = new Operator[numberOfOperators];
            for(int i = 0; i < numberOfOperators; i++)
            {
                operators[i]=new Operator(maxQueueLength);
            }
            
            Random random = new Random();

            // Функция генерации времени до нового запроса
            int timeToNewRequest(int min, int max) { return random.Next(2, 9); };
            // Функция генерации времени новой задачи
            int newTaskTime(int min, int max ) { return random.Next(10, 51); };

            int denied = 0;
            for (int i = 1; i < callsIteration+1; i++)
            {
                int time = timeToNewRequest(minTimeToNewRequest, maxTimeToNewRequest);
                Console.WriteLine("Tick time:" + time);
                for (int k = 0; k < operators.Length; k++)
                {
                    operators[k].Tick(time);
                    Console.WriteLine($"{k + 1} operator task now:{operators[k].CurrentTask} queue langth:{operators[k].QueueLength} load coefficient:{Math.Round(operators[k].LoadCoefficient, 2)}");
                }
                try
                {
                    // Получение идентификатора оператора с наименьшей загрузкой
                    int minLoadId = getMinLoadId(operators);
                    int TaskTime = newTaskTime(minTaskTime, maxTaskTime);
                    if (minLoadId >= 0 && operators[minLoadId].TryAddNewTask(TaskTime))
                    {
                        Console.WriteLine($"{i}  CALL WAS RECEIVED AND DISTRIBUTED FOR {minLoadId + 1} OPERATOR, TASK TIME:{TaskTime}".ToUpper());
                    }
                    else
                    {
                        Console.WriteLine($"{i} itaration denied".ToUpper());
                        denied++;
                    }
                    Console.WriteLine($"----------------------------------------");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine($"all denied calls:{denied}\n");
            Console.WriteLine($"all сompleted calls:{callsIteration-denied}\n");

            for (int k = 0; k < operators.Length; k++)
            {
                int totalWorkTime = operators[k].TotalWorkTime;
                int taskСompleted = operators[k].TaskСompleted;
                foreach (var task in operators[k].TaskQueue)
                {
                    taskСompleted++;
                    totalWorkTime += task;
                }
                if(operators[k].CurrentTask>0) taskСompleted++;
                totalWorkTime += operators[k].CurrentTask;
                double loadCoefficient = operators[k].LoadCoefficient;
                Console.WriteLine($"Operator {k + 1} load coefficient: {Math.Round(loadCoefficient, 2)}");
                Console.WriteLine($"Operator {k + 1} task сompleted: {taskСompleted}");
                Console.WriteLine($"Operator {k + 1} total work time : {totalWorkTime}");
                Console.WriteLine($"Operator {k + 1} total idle time : {operators[k].TotalIdleTime}\n");
            }
            complexDenied += denied;
        }
        Console.WriteLine($"\nсреднее за {complexIteration}:{complexDenied / complexIteration}");
    }

    // Получение идентификатора оператора с наименьшей загрузкой
    static int getMinLoadId(Operator[] operators)
    {
        int minLoadId = -1;
        double minLoadCoefficient = double.MaxValue;
        int minQueueLength = int.MaxValue;

        // Проверяем свободных операторов
        for (int i = 0; i < operators.Length; i++)
        {
            if (operators[i].CurrentTask == 0)
            {
                // Если оператор свободен, но его коэффициент загрузки определен, то выбираем его
                double loadCoefficient = operators[i].LoadCoefficient;
                if (!double.IsNaN(loadCoefficient) && loadCoefficient < minLoadCoefficient)
                {
                    minLoadCoefficient = loadCoefficient;
                    minLoadId = i;
                }
            }
        }
        if (minLoadId != -1)
        {
            return minLoadId;
        }
        minQueueLength = int.MaxValue;
        minLoadCoefficient = double.MaxValue;
        // Проверяем остальных операторов
        for (int i = 0; i < operators.Length; i++)
        {
            if (!operators[i].IsQueueFull)
            {
                int queueLength = operators[i].QueueLength;
                double loadCoefficient = operators[i].LoadCoefficient;

                if (queueLength < minQueueLength || (queueLength == minQueueLength && loadCoefficient < minLoadCoefficient))
                {
                    minQueueLength = queueLength;
                    minLoadCoefficient = loadCoefficient;
                    minLoadId = i;
                }
            }
        }

        return minLoadId;
    }

}
