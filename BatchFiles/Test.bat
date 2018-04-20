SET here=%~dp0

IF NOT EXIST "%here%done.txt" (
	IF EXIST "%here%update.txt" (
		echo. 2>ImUpdating.txt
	) ELSE (
		echo. 2>ImInserting.txt
	IF NOT EXIST "%here%update.txt" (
		echo. 2>done.txt
	)
	)
) ELSE (
	echo. 2>ImDone.txt
)
