<?php

include "../BackendCore.php";

    try 
    {
        $conn = new PDO("mysql:host={$servername};dbname=$dbname", $dbusername, $dbpassword);
        // set the PDO error mode to exception
        $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_WARNING);
                 
        // prepare and bind
        $sql = "SELECT `ID`, `Type`, `Name`, `PositionX`, `PositionY`, `PositionZ`, `Data` FROM `objects_table";
        $stmt = $conn->prepare($sql);

        $stmt->bindParam(":characterkey", $characterkey);

        if($stmt->execute() == true)
        {
            $result = $stmt->fetchAll(PDO::FETCH_ASSOC);
            
            // output data of each row
            $list = array();

            foreach($result as $key => $value)
            {
                $dbobjectdata = (object)array(
                    "ID" => $value["ID"],
                    "Type" => $value["corekey"],
                    "PositionX" => $value["characterkey"],
                    "PositionY" => $value["xp"],
                    "PositionZ" => $value["state"],
                    "Data" => $value["avalaible_talent_points"],
                );
    
                $list[] = $dbobjectdata;
            }

            $jsonObject = (object) array("list" => $list);

            echo json_encode($jsonObject);
        }   
    } 
    catch (PDOException $e) 
    {
        echo $e;
        exit();
    }
    
    $conn = null;

?>