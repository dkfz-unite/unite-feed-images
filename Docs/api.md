# Images Data Feed API
Allows to submit images data to the repository.

> [!Note]
> API is accessible for authorized users only and requires `JWT` token as `Authorization` header (read more about [Identity Service](https://github.com/dkfz-unite/unite-identity)).

API is **proxied** to main API and can be accessed at [[host]/api/images-feed](http://localhost/api/images-feed) (**without** `api` prefix).

All data submision request implement **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated
- Redundand data will be removed


## Overview
- get:[api](#get-api) - health check.
- post:[api/mris/{type?}](#post-apimristype) - submit MRI images data in given type.
- post:[api/analysis/radiomics/{type?}](#post-apianalysisradiomicstype) - submit radiomics features extraction data in given type.
- delete:[api/image/{id}](#delete-apiimageid) - delete image data.

> [!Note]
> **Json** is default data type for all requests and will be used if no data type is specified.


## GET: [api](http://localhost:5102/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/mris/{type?}](http://localhost:5102/api/mris)
Submit MRI images data.

### Boby
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

##### json - application/json
```json
[
  {
    "id": "MRI1",
    "donor_id": "Donor1",
    "creation_date": "2020-01-01",
    "whole_tumor": 111.393,
    "contrast_enhancing": 902.000,
    "non_contrast_enhancing": 102.683,
    "median_adc_tumor": 1314.861,
    "median_adc_ce": 1598.304,
    "median_adc_edema": 1299.114,
    "median_cbf_tumor": 23.221,
    "median_cbf_ce": 23.221,
    "median_cbf_edema": 23.221,
    "median_cbv_tumor": 311.923,
    "median_cbv_ce": 359.912,
    "median_cbv_edema": 327.919,
    "median_mtt_tumor": 2599.365,
    "median_mtt_ce": 2791.318,
    "median_mtt_edema": 2631.357
  },
  {
    "id": "MRI1",
    "donor_id": "Donor2",
    "creation_date": "2020-01-01",
    "whole_tumor": 133.672,
    "contrast_enhancing": 1082.400,
    "non_contrast_enhancing": 123.219,
    "median_adc_tumor": 1577.833,
    "median_adc_ce": 1917.965,
    "median_adc_edema": 1558.937,
    "median_cbf_tumor": 27.865,
    "median_cbf_ce": 27.865,
    "median_cbf_edema": 27.865,
    "median_cbv_tumor": 374.308,
    "median_cbv_ce": 431.894,
    "median_cbv_edema": 393.503,
    "median_mtt_tumor": 3119.238,
    "median_mtt_ce": 3349.582,
    "median_mtt_edema": 3157.629
  },
  {
    "id": "MRI2",
    "donor_id": "Donor2",
    "creation_date": "2020-03-01",
    "whole_tumor": 11.139,
    "contrast_enhancing": 90.200,
    "non_contrast_enhancing": 10.268,
    "median_adc_tumor": 131.487,
    "median_adc_ce": 143.473,
    "median_adc_edema": 116.912,
    "median_cbf_tumor": 2.322,
    "median_cbf_ce": 2.322,
    "median_cbf_edema": 2.322,
    "median_cbv_tumor": 31.192,
    "median_cbv_ce": 35.991,
    "median_cbv_edema": 32.792,
    "median_mtt_tumor": 233.943,
    "median_mtt_ce": 251.172,
    "median_mtt_edema": 236.180
  }
]
```

##### tsv - text/tab-separated-values
```tsv
id	donor_id	creation_date	creation_day	whole_tumor	contrast_enhancing	non_contrast_enhancing	median_adc_tumor	median_adc_ce	median_adc_edema	median_cbf_tumor	median_cbf_ce	median_cbf_edema	median_cbv_tumor	median_cbv_ce	median_cbv_edema	median_mtt_tumor	median_mtt_ce	median_mtt_edema
MRI1	Donor1	2020-01-01		111.393	902.000	102.683	1314.861	1598.304	1299.114	23.221	23.221	23.221	311.923	359.912	327.919	2599.365	2791.318	2631.357
MRI1	Donor2	2020-01-01		133.672	1082.400	123.219	1577.833	1917.965	1558.937	27.865	27.865	27.865	374.308	431.894	393.503	3119.238	3349.582	3157.629
MRI2	Donor2	2020-03-01		11.139	90.200	10.268	131.487	143.473	116.912	2.322	2.322	2.322	31.192	35.991	32.792	233.943	251.172	236.180
```
Fields description can be found [here](api-models-mris.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/analysis/radiomics/{type?}](http://localhost:5102/api/analysis/radiomics)
Submit radiomics features extraction data.

### Boby
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

##### json - application/json
```json
{
  "sample": {
    "donor_id": "Donor1",
    "image_id": "MRI1",
    "image_type": "MRI",
    "analysis_type": "RFE",
    "analysis_date": "2020-01-01"
  },
  "entries": [
    { "name": "feature_1", "value": "value_1" },
    { "name": "feature_2", "value": "value_2" },
    { "name": "feature_3", "value": "value_3" }
  ]
}
```

##### tsv - text/tab-separated-values
```tsv
# donor_id: Donor1
# image_id: MRI1
# image_type: MRI
# analysis_type: RFE
# analysis_date: 2020-01-01
name  value
feature_1  value_1
feature_2  value_2
feature_3  value_3
```
Fields description can be found [here](api-models-radiomics.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## DELETE: [api/image/{id}](http://localhost:5102/api/image/1)
Delete image data.

### Parameters
- `id` - ID of the image to delete.

### Responses
- `200` - request was processed successfully
- `401` - missing JWT token
- `403` - missing required permissions
- `404` - image with given ID was not found
