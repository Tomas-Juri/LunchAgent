SET here=%~dp0

IF NOT EXIST "%here%done.txt" (
	IF EXIST "%here%update.txt" (
		LunchAgent_Console.exe JsonData\RestaurantConfig.json JsonData\SlackConfig.json 1
	) ELSE (
		LunchAgent_Console.exe JsonData\RestaurantConfig.json JsonData\SlackConfig.json 2
	)
)
