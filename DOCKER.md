## білд зображення

```powershell
docker build -t tamagotchi-game .
```

## запуск гри

```powershell
docker run --rm -it tamagotchi-game
```

`-it` параметр необхідний для того щоб гра могла зчитувати клавіатуру в інтерактивному терміналі

## запуск з docker compose

```powershell
docker compose run --rm tamagotchi
```

## запуск тестів локально

```powershell
dotnet test tests/consoletamagotchi.tests.csproj
```

## що було створено для 38 лабораторної

було створено:
- [`Dockerfile`](Dockerfile);
- [`.dockerignore`](.dockerignore);
- [`compose.yaml`](compose.yaml);
- [`DOCKER.md`](DOCKER.md).