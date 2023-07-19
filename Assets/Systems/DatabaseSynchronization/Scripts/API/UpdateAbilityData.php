<?php

include "../BackendCore.php";

$headers = apache_request_headers();

$userkey = $headers["JWTUSERKEY"];
$tokenkey = $headers["JWTKEY"];

$jsondata = file_get_contents('php://input');
$data = json_decode($jsondata);

if(CheckToken($userkey, $tokenkey))
{
    try{

        $conn = new PDO("mysql:host={$servername};dbname=$dbname", $dbusername, $dbpassword);
        // set the PDO error mode to exception
        $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_WARNING);
          
        $sql = "UPDATE 
                `characters_abilities` 
                SET 
                `xp` = :xp, 
                `state` = :abilitystate, 
                `selected_talents` = :selected_talents, 
                `total_talent_points` = :total_talent_points, 
                `avalaible_talent_points` = :avalaible_talent_points, 
                `unlocked_talents` = :unlocked_talents, 
                `rune_corekey` = :rune_corekey 
                WHERE 
                characterkey = :characterkey AND corekey = :corekey ";

        $stmt = $conn->prepare($sql);
        $stmt->bindParam(":characterkey", $data->{'characterKey'});
        $stmt->bindParam(":corekey", $data->{'coreKey'});

        $stmt->bindParam(":xp", $data->{'xp'});
        $stmt->bindParam(":abilitystate", $data->{'state'});

        $jsonselectedtalents = json_encode($data->{'selectedTalents'});
        $jsonunlockedtalents = json_encode($data->{'unlockedTalents'});
        $stmt->bindParam(":selected_talents", $jsonselectedtalents);
        $stmt->bindParam(":unlocked_talents", $jsonunlockedtalents);

        $stmt->bindParam(":total_talent_points", $data->{'totalTalentPoints'});
        $stmt->bindParam(":avalaible_talent_points", $data->{'avalaibleTalentPoints'});
        $stmt->bindParam(":rune_corekey", $data->{'runeKey'});

        if($stmt->execute() === true)
        {
            echo "Updated ability succesfuly";
        }
        else
        {
            echo BackendError(1, "UpdateAbilityData", "Couldn't execute query : ");
        }

        $conn = null;
    }
    catch(Exception $e)
    {
        echo BackendError(2, "UpdateAbilityData", $e);
        exit();
    }
}

?>