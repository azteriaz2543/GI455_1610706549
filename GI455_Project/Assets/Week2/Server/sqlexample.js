const sqlite = require('sqlite3').verbose();

var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE |sqlite.OPEN_READWRITE, (err) =>{

    if(err) throw err;

    console.log("Connected to database");

    var userID = "test0006";
    var password = "1";
    var name = "Test6";

    var sqlSelect = "SELECT UserID,Name FROM UserData" //เรียกหาข้อมูล
    var sqlInsert = `INSERT INTO UserData (UserID ,Password ,Name) VALUES ('${userID}','${password}','${name}')` //เพิ้มเข้าไปในดาต้า
    var sqlUpdate = "UPDATE UserData SET Money = 1000000 WHERE UserID = 'test0006'" //Update

    database.all(sqlSelect,(err, rows)=>{

        if(err){
                console.log(err);
        }else{
            /*
            if(rows.length > 0)
            {
                var currentMoney = rows[0].Money;
                currentMoney += 100;

                database.all("UPDATE UserData SET Money = '"+currentMoney+"' WHERE UserID = '"+userID+"'"),(err, rows)=>
                {

                }
            }else{
                console.log("UserID not found");
            }
                
            */
                console.log(rows);
        }

    });

});
