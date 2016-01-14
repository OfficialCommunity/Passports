
    $root =                               $psake.build_script_dir
    $solution_file_common =               "$root/Src/OCC.Passports.Common.sln"   
	$solution_file_common_web =           "$root/Src/OCC.Passports.Common.Web.sln"  
	$solution_file_Storage_Logentries =   "$root/Src/OCC.Passports.Storage.Logentries.sln"  
	$solution_file_Storage_null =        "$root/Src/OCC.Passports.Storage.Null.sln" 	
    $configuration =                      "Release"
    $build_dir =                          "$root\Build\"   





    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue | Out-Null



    new-item $build_dir -itemType directory | Out-Null
  msbuild "$solution_file_common" /m /p:OutDir=$build_dir /p:Configuration=$configuration 
   msbuild "$solution_file_common_web" /m /p:OutDir=$build_dir /p:Configuration=$configuration 
 msbuild "$solution_file_Storage_Logentries" /m /p:OutDir=$build_dir /p:Configuration=$configuration 
     msbuild "$solution_file_Storage_null" /m /p:OutDir=$build_dir /p:Configuration=$configuration 	


