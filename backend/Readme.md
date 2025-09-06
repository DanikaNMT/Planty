## Quickstart

### Build the solution
```
dotnet build
```

### Run the API
```
cd src/PlantApp.API
dotnet run
```

### Run tests
```
dotnet test
```

### Migrations
```
dotnet ef migrations add InitialCreate
```


### Publishing
E.g. when being hosted on a linux-arm64 server. 
```
dotnet publish src/PlantApp.API/PlantApp.API.csproj -c Release -r linux-arm64 --self-contained -o publish/

scp -r publish/ name@ip:~/planty/
```

On server:
```
sudo nano /etc/systemd/system/planty.service
```
```
sudo systemctl enable plantapp
sudo systemctl start plantapp
sudo systemctl status plantapp
```