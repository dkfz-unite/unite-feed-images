# Image Analysis Data Model

## Analysis
Includes data about image analysis

**`id`*** - Image analysis identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"IM1AN1"`

**`type`** - Image analysis type.
- Type: _String_
- Possible values: `"RFE"`
- Example: `"RFE"`

**`parameters`** - Image analysis parameters.
- Type: _Object(Dictionary)_
- Key type: _String_
- Key limitations: Maximum length 255
- Value type: _String_
- Value limitations: Not null
- Limitations - If set, at least one field has to be set; all field should be unique
- Example: `{ "Key1": "Value1" }`

**`features`** - Image features revealed during the analysis.
- Type: _Object(Dictionary)_
- Key type: _String_
- Key limitations: Maximum length 255
- Value type: _String_
- Value limitations: Not null
- Limitations - If set, at least one field has to be set; all fields should be unique
- Example: `{ "Key1": "Value1" }`

#### Analysis Type
Supported analysis types are:
- `"RFE"` - Radiomics Feature Extraction

##
**`*`** - Required fields
