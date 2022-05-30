"use strict";

const db=require("../config/db");

class UserStorage{
    static #getUserInfo(data, id){
        const users=JSON.parse(data);
        const idx=users.id.indexOf(id);
        const usersKeys=Object.keys(users);
        const userInfo=userKeys.reduce((newUser, info) => {
            newUser[info]=users[info][idx];
            return newUser;
        }, {});

        return userInfo;        
    }

    static #getUsers(data, isAll, fields){
        const users=JSON.parse(data);
        if(isAll) return users;

        const newUsers=fields.reduce((newUsers, field) => {
            if(users.hasOwnProperty(field)){
                newUsers[field]=users[field];
            }
            return newUsers;
        }, {});
        return newUsers;
    }

    static getUsers(isAll, ...fields) {}

    static getUserInfo(id) {
        
    }

    static async save(userInfo) {}
}

module.exports=UserStorage;