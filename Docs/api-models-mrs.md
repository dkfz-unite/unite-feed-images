# MR Image Data Model
Includes all basic [image](./api-models-image.md) information.

**`whole_tumor`** - Whole tumor volume in cm³.
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `111.393`

**`contrast_enhancing`** - Contrast enhancing volume in cm³.
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `902.000`

**`non_contrast_enhancing`** - Non contrast enhancing volume in cm³.
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `102.683`

**`median_adc_tumor`** - Median apparent diffusion coefficient (whole tumor).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `1314.861`

**`median_adc_ce`** - Median apparent diffusion coefficient (contrast enhancing part).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `1598.304`

**`median_adc_edema`** - Median apparent diffusion coefficient (edema).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `1299.114`

**`median_cbf_tumor`** - Median cerebral blood flow (whole tumor).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `23.221`

**`median_cbf_ce`** - Median cerebral blood flow (contrast enhancing part).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `23.221`

**`median_cbf_edema`** - Median cerebral blood flow (edema).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `23.221`

**`median_cbv_tumor`** - Median cerebral blood volume (whole tumor).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `311.923`

**`median_cbv_ce`** - Median cerebral blood volume (contrast enhancing part).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `359.912`

**`median_cbv_edema`** - Median cerebral blood volume (edema).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `327.919`

**`median_mtt_tumor`** - Median mean transit time tumor.
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `2599.365`

**`median_mtt_ce`** - Median mean transit time (contrast enhancing part).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `2791.318`

**`median_mtt_edema`** - Median mean transit time (edema).
- Type: _Number_
- Limitations: Float, greater or equal to 0
- Example: `2631.357`
