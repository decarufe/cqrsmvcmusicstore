﻿ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [DEMO_MVC_log], FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\DEMO_MVC_log.LDF', SIZE = 576 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

