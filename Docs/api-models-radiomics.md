# Radiomocs Features Extraction Analusis Data Model
Includes information about analysed sample and radiomics features data.

**`sample`*** - Image sample and it's analysis data.
- Type: [_Object_](./api-models-base-sample.md)
- Example: `{...}`

**`entries`*** - List of extracted radiomics features.
- Type: _Array_
_ Element type: [_Object_](#radiomics-feature)
- Limitations: Should contain at least one element.
- Example: `[{...}, {...}]`


### Radiomics Feature
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
