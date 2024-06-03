# Radiomocs Features Extraction Analusis Model
Includes data about radiomics features extraction analysis and it's results.

**`sample`*** - Image sample and it's analysis data.
- Type: [_Object_](./api-models-base-sample.md)
- Example: `{...}`

**`entries`*** - List of extracted radiomics features.
- Type: _Array_
_ Element type: [_Object_](#entry)
- Example: `[{...}, {...}]`


### Entry
Radiomics feature entry.

**`name`*** - Feature name.
- Type: _String_
_ Limitations: Max length 255
- Example: `"feature_1"`

**`value`*** - Feature value.
- Type: _String_
- Example: `"value_1"`


##
**`*`** - Required fields
