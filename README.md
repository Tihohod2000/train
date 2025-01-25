Решение тестового задания

Репозиторий содержит 3 проекта:
1. gRPC service
2. gRPC client console
3. gRPC client UI

Для корректной работы: 
1. Укажите правильное подключение gRPC service к БД в файле: ```train/GrpcService1/GrpcStationService/appsetting.json```
 Измените настройки подключения на свои тут:
```
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Username=postgres;Password=12345;Database=Wagons"
  }
```  
2. Соберите проект
3. БД должна содержать в себе таблицы: Epc, EpcEvent, EventArrival, EventDeparture, Path.
   
   Таблица ЕПС (Epc). Хранит информацию о единицах подвижного состава.
   
   Таблица События ЕПС (EpcEvent). Хранит информацию о всех событиях на станции с единицами подвижного состава.
   
   Таблица События прибытия (EventArrival). Хранит информацию о прибытии на станцию.
   
   Таблица События отправления (EventDeparture). Хранит информацию об отправлениях со станции.
   
   Таблица пути (Path). Хранит информацию о путях на станции.
