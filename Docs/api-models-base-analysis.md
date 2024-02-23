# Image Analysis Data Model
Includes data about image analysis.

>[!NOTE]
> All exact dates are hiddent and protected. Relative dates are shown instead, if calculation was possible.

**`id`*** - Image analysis identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"MRI1-Analysis1"`

**`type`** - Image analysis type.
- Type: _String_
- Possible values: `"RFE"`
- Example: `"RFE"`

**`date`** - Image analysis date.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'date' or 'day' can be set at once, not both 
- Example: `"2019-01-01"`

**`day`** - Relative number of days since diagnosis statement, when analysis was performed.
- Type: _Number_
- Limitations:  Integer, greater or equal to 1, only either 'date' or 'day' can be set at once, not both
- Example: `1`

**`parameters`** - Image analysis parameters.
- Type: _Object(Dictionary)_
- Key type: _String_
- Key limitations: Maximum length 255
- Value type: _String_
- Value limitations: Not null
- Limitations - If set, at least one field has to be set; all field should be unique
- Example: `{ "key1": "value1" }`

**`features`*** - Image features revealed during the analysis.
- Type: _Object(Dictionary)_
- Key type: _String_
- Key limitations: Maximum length 255
- Value type: _String_
- Value limitations: Not null
- Limitations - If set, at least one field has to be set; all fields should be unique
- Example: `{ "key1": "value1" }`

#### Analysis Type
Supported analysis types are:
- `"RFE"` - Radiomics Feature Extraction

##
**`*`** - Required fields
