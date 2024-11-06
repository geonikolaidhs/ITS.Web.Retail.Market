rem Go to the bat file's folder regardless of where the script was called from
cd /d "%~dp0"
xcopy "master_test_config.json" "..\Tests\WRM\Admin\Master\test_config.json" /y
xcopy "master_test_config.json" "..\Tests\WRM\Customer\Master\test_config.json" /y
xcopy "master_test_config.json" "..\Tests\WRM\Supplier\Master\test_config.json" /y
xcopy "storecontroller_test_config.json" "..\Tests\WRM\Admin\StoreController\test_config.json" /y
xcopy "dual_test_config.json" "..\Tests\WRM\Admin\Dual\test_config.json" /y
