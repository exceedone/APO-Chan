**Get Report**
----
  Get reports list.

* **URL**

  /tables/report

* **Method:**
  
  `GET`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**

   **Optional:**
   `Id=[Filtering report GUID]`<br/>
   `RefUserId=[Filtering GUID report created user]`<br/>
   `Year=[Filtering year of ReportStartDate or year of ReportEndDate]`<br/>
   `Month=[Filtering month of ReportStartDate or month of ReportEndDate]`

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
  
  
  
  
**Insert Report**
----
  Insert report data into database.

* **URL**

  /tables/report

* **Method:**
  
  `POST`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**
   `RefUserId=[Set GUID report created user]`<br/>
   `ReportTitle=[Set Report Title]`<br/>
   `ReportComment=[Set Report Comment]`<br/>
   `ReportComment=[Set Report Comment]`<br/>
   `ReportStartDate=[Set Report StartDate]`<br/>
   `ReportStartTime=[Set Report StartTime]`<br/>
   `ReportEndDate=[Set Report EndDate]`<br/>
   `ReportEndTime=[Set Report EndTime]`<br/>
   `ReportLat=[Set Latitude where users post report]`<br/>
   `ReportLon=[Set Longitude where users post report]`
   
   **Optional:**
 
   `Id=[Set GUID if you want to set id expressly]`

* **Data Params**

  None

* **Success Response:**
  
  * **Code:** 200
    **Content:** 
        {
          "refUserId": "cfc17591-37d5-4fd0-bf1b-1fa075d96817",
          "reportTitle": "test data 5",
          "reportComment": "comment5",
          "reportStartDate": "2017-09-10T00:00:00Z",
          "reportStartTime": "10:00:00",
          "reportEndDate": "2017-09-10T00:00:00Z",
          "reportEndTime": "11:00:00",
          "reportLat": 35.93621947,
          "reportLon": 140.034237,
          "id": "5b71111cce2d4ef0bf4afac1bc3aac90",
          "version": "AAAAAAAAB+A=",
          "createdAt": "2017-08-14T05:26:10.752Z",
          "updatedAt": "2017-08-14T05:26:10.752Z",
          "deleted": false
        }
 
* **Error Response:**
  Now Developing.
  
  
  
  
**Update Report**
----
  Update report data.

* **URL**

  /tables/report/:id

* **Method:**
  
  `PATCH`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**
   `:id=[Set GUID]`

   **Optional:**
   `RefUserId=[Set GUID report created user]`<br/>
   `ReportTitle=[Set Report Title]`<br/>
   `ReportComment=[Set Report Comment]`<br/>
   `ReportComment=[Set Report Comment]`<br/>
   `ReportStartDate=[Set Report StartDate]`<br/>
   `ReportStartTime=[Set Report StartTime]`<br/>
   `ReportEndDate=[Set Report EndDate]`<br/>
   `ReportEndTime=[Set Report EndTime]`<br/>
   `ReportLat=[Set Latitude where users post report]`<br/>
   `ReportLon=[Set Longitude where users post report]`

* **Data Params**

  None

* **Success Response:**
  
  * **Code:** 200
    **Content:** 
        #TODO
 
* **Error Response:**
  Now Developing.


  
**Delete Report**
----
  Delete report data.
  * Now execute physical delete, but someday we have to develop as logical delete.

* **URL**

  /tables/report/:id

* **Method:**
  
  `DELETE`
  
*  **URL Params**

   Write common required parameter. Please read "Common parameter" reference.
   
   **Required:**
   `:id=[Set GUID]`

   **Optional:**

* **Data Params**

  None

* **Success Response:**
  
  * **Code:** 200
    **Content:** 
        #TODO
 
* **Error Response:**
  Now Developing.