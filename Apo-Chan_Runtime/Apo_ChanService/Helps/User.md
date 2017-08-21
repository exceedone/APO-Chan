**Get User**
----
  Get users list.

* **URL**

  /tables/user

* **Method:**
  
  `GET`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**

   **Optional:**
   `Id=[Filtering user GUID]`<br/>
   `ProviderType=[Filterring provider type user signined]`<br/>
   `UserProviderId=[Filterring user provider id]`
   

* **Data Params**

  None

* **Success Response:**
  
  * **Code:** 200
    **Content:** 
        [{"createdAt": "2017-08-10T07:51:36.373Z", 
        "deleted": false, 
        "deletedAt": null, 
        "id": "9be14d3352b14a63862260a6009d5edb", 
        "refUserId": "cfc17591-37d5-4fd0-bf1b-1fa075d96817", 
        "reportComment": "comment1", 
        "reportEndDate": "2017-08-10T00:00:00Z", 
        "reportEndTime": "11:00:00", 
        "reportLat": 35.6959506, 
        "reportLon": 139.7593487, 
        "reportStartDate": "2017-08-10T00:00:00Z", 
        "reportStartTime": "10:00:00", 
        "reportTitle": "test data 1", 
        "updatedAt": "2017-08-10T07:51:36.388Z", 
        "version": "AAAAAAAAB9s="}]
 
* **Error Response:**
  Now Developing.
  
  
  
  
**Insert User**
----
  Insert user data into database.

* **URL**

  /tables/user

* **Method:**
  
  `POST`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**
   `ProviderType=[Set provider type user signined]`<br/>
   `UserProviderId=[Set user signined provider id]`<br/>
   `Email=[Set user email]`<br/>
   `UserName=[Set user name]`

   **Optional:**
 
   `Id=[Set GUID if you want to set id expressly]`

* **Data Params**

  None

* **Success Response:**
  
  * **Code:** 200
    **Content:** 
        {"providerType": "Google",
        "userProviderId": "359027072884000",
        "userName": null,
        "email": "test2@test.jp",
        "deletedAt": null,
        "id": "dc9496e6-753b-4e25-80cb-631ad2f1cf5c",
        "version": "AAAAAAAAB94=",
        "createdAt": "2017-08-14T00:50:56.904Z",
        "updatedAt": "2017-08-14T00:50:56.904Z",
        "deleted": false
        }
 
* **Error Response:**
  Now Developing.
  
  
  
  
**Update User**
----
  Update user data.

* **URL**

  /tables/user/:id

* **Method:**
  
  `PATCH`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**
   `:id=[Set GUID]`

   **Optional:**
   `ProviderType=[Set provider type user signined]`<br/>
   `UserProviderId=[Set user signined provider id]`<br/>
   `Email=[Set user email]`<br/>
   `UserName=[Set user name]`

* **Data Params**

  None

* **Success Response:**
  
  * **Code:** 200
    **Content:** 
        {"providerType": "Google",
        "userProviderId": "359027072884000",
        "userName": null,
        "email": "test2@test.jp",
        "deletedAt": null,
        "id": "dc9496e6-753b-4e25-80cb-631ad2f1cf5c",
        "version": "AAAAAAAAB94=",
        "createdAt": "2017-08-14T00:50:56.904Z",
        "updatedAt": "2017-08-14T00:50:56.904Z",
        "deleted": false
        }
 
* **Error Response:**
  Now Developing.

  
  
**Delete User**
----
  Delete user data.

* **URL**

  /tables/user/:id

* **Method:**
  
  `DELETE`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**
   `:id=[Set GUID]`

* **Data Params**

  None

* **Success Response:**
  
  * **Code:** 200
    **Content:** 
        {}
 
* **Error Response:**
  Now Developing.