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
3. Соберите проект
