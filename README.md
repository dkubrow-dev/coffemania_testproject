# coffemania_testproject
Тестовое задание от кофемании от 30.03.2026: "необходимо реализовать проект расчета расстояния в километрах между двумя точками , точки передаются как входные параметры в запросе. код проекта отдать или архивом или ссылка на гитхаб"

# Что делает проект?

Считает расстояние в километрах между двумя точками. На вход принимает 2 точки плоскости. Входные параметры: две точки на плоскости, заданные в double формате. Выходные данные: расстояние в double формате, либо сообщение об ошибке.

Считается прямолинейное расстояние между точками, без поиска пути и учёта препятствий.

Проект может быть использован двумя способами. 1) Подключаемая .NET-библиотека (проект подготовлен к упаковке в NuGet-пакет); 2) как сервис с Web API интерфейсом.

# Как запустить?

## Вариант 1. Как библиотеку .Net

Достаточно скачать библиотеку и сослаться на неё из своего проекта .Net. Затем используйте публичные методы:

``` C#
// используйте пространства имён проекта
using DistanceCalc.Abstractions;
using DistanceCalc.Models;
using DistanceCalc.Services;

// Реализуйте ISettingsProvider для настроек сервиса
sealed record SettingsProvider(CalculatorServiceModes Mode, int SimulateDelayMs) : ISettingsProvider;

// Получите из фабрики соответствующий калькулятор
IDistanceCalculationService calculator = CalculatorsFactory.GetInstance(new SettingsProvider(CalculatorServiceModes.DirectLine, -1));

// Сформируйте входящий объект
Input input = new() { PointA = new(0, 1), PointB = new(3, 4) };

// Получите 
using CancellationTokenSource cancellationTokenSource = new();
CancellationToken cancellationToken = cancellationTokenSource.Token;
Result result = await calculator.CalculateAsync(input, cancellationToken);

```

## Вариант 2. Как Web API

Для Windows развернуть решение на сервере и зарегистрировать exe как службу. Например, PowerShell:

```
New-Service -Name DistanceWebApi `
  -BinaryPathName "C:\Apps\DistanceWebApi\DistanceWebApi.exe --contentRoot C:\Apps\DistanceWebApi" `
  -DisplayName "Distance Web API" `
  -StartupType Automatic
Start-Service -Name DistanceWebApi
Get-Service -Name DistanceWebApi

```

Для Linux развернуть решение на сервере, зарегистрировать systemd unit. Например:

```
[Unit]
Description=Distance Web API

[Service]
WorkingDirectory=/var/www/distanceapi
ExecStart=/usr/bin/dotnet /var/www/distanceapi/DistanceWebApi.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=distanceapi
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5000
Type=notify

[Install]
WantedBy=multi-user.target

```

Потом в Bash:

```
sudo systemctl daemon-reload
sudo systemctl enable distanceapi.service
sudo systemctl start distanceapi.service
sudo systemctl status distanceapi.service
sudo journalctl -fu distanceapi.service
```

## Архитектура и упрощения

Сознательно упрощено:

- фактический расчёт расстояния между точками до банальной теоремы Пифагора;
- управление настройками подробно не прорабатывалось;
- развёртывание Web API в системе вручную, без контейнера;
- отсутствуют проверки состояния сервиса для мониторинга.

Архитектурные решения и причины упрощений: /documentation/Architecture.docx

## Как искать проблемы?

Логи ведутся в следующих местах:
- в DistanceCalc - технические логи для расчёта самого расстояния; путь - /logs/distancecalc/
- в DistanceWebApi - логи получаемых и отправялемых запросов HTTP и контекста исполнения; путь - /logs/webapi/

