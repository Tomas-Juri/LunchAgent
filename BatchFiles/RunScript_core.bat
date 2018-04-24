SET here=%~dp0

IF NOT EXIST "%here%done.txt" (
	IF EXIST "%here%update.txt" (
		dotnet LunchAgent.dll JsonData\RestaurantConfig.json JsonData\SlackBotKey.txt 1
	) ELSE (
		dotnet LunchAgent.dll JsonData\RestaurantConfig.json JsonData\SlackBotKey.txt 2
	)
)
