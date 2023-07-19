<?php

include "BackendCore.php";

header("Access-Control-Allow-Headers: Authorization, Content-Type");
header("Access-Control-Allow-Origin: *");
header('content-type: application/json; charset=utf-8');

$headers = apache_request_headers();

$jsondata = file_get_contents('php://input');
echo $jsondata;
$data = (object)json_decode($jsondata);

echo $data->{'Type'};

    try{

        $conn = new PDO("mysql:host={$servername};dbname=$dbname", $dbusername, $dbpassword);
        // set the PDO error mode to exception
        $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_WARNING);
          
        $sql = "UPDATE 
                `objects_table` 
                SET 
                `Type` = :_Type, 
                `Name` = :_Name, 
                `PositionX` = :PositionX, 
                `PositionY` = :PositionY, 
                `PositionZ` = :PositionZ, 
                `Data` = :_Data
                WHERE 
                ID = :ID";

        $stmt = $conn->prepare($sql);
        $stmt->bindParam(":ID", $data->{'ID'});
        $stmt->bindParam(":_Type", $data->{'Type'});
        $stmt->bindParam(":_Name", $data->{'Name'});
        $stmt->bindParam(":PositionX", $data->{'PositionX'});
        $stmt->bindParam(":PositionY", $data->{'PositionY'});
        $stmt->bindParam(":PositionZ", $data->{'PositionZ'});
        $stmt->bindParam(":_Data", $data->{'Data'});

        echo "TOTO";

        if($stmt->execute() === true)
        {
            echo "Updated ability succesfuly";
        }
        else
        {
            echo "Error";
        }

        $conn = null;
    }
    catch(Exception $e)
    {
        echo $e;
        exit();
    }

?>