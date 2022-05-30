const mysql=require("mysql");

const db=mysql.createConnection({
    host: "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com",
    user: "admin",
    password: "dbjunohshin",
    database: "plus2048",
});

db.connect();

module.exports=db;