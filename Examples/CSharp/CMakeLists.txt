
if(NOT BUILD_TESTING)
  return()
endif()

#
# CSharp Examples
#

sitk_add_csharp_test( Example.ImageGetSetBuffer
  "${CMAKE_CURRENT_SOURCE_DIR}/ImageGetSetBuffer.cs"
  DATA{${SimpleITK_DATA_ROOT}/Input/RA-Short.nrrd})
set_tests_properties( CSharp.Example.ImageGetSetBuffer
  PROPERTIES PASS_REGULAR_EXPRESSION "total: 1009713" )

sitk_add_csharp_test( Example.ImageGetSetBuffer
  "${CMAKE_CURRENT_SOURCE_DIR}/ImageGetSetBuffer.cs"
  DATA{${SimpleITK_DATA_ROOT}/Input/RA-Short.nrrd})
  "${SimpleITK_TEST_OUTPUT_DIR}/CSharp.SetBuffer.png"
  IMAGE_MD5_COMPARE
    "${SimpleITK_TEST_OUTPUT_DIR}/CSharp.SetBuffer.png"    "80d3be0e7a9cafad1ec4fca5bc5cfe89c8a416bc203efccd49cfaf177fb88a4e7e76a336bb09fc2aedca2522a0911e72622a4cde5888b0ec6e415e49d977845a"
  )

  sitk_add_csharp_test( Example.ConnectedThresholdSegmentation
  "${CMAKE_CURRENT_SOURCE_DIR}/ConnectedThresholdSegmentation.cs"
  DATA{${SimpleITK_DATA_ROOT}/Input/BrainProtonDensitySlice.png}
  "${SimpleITK_TEST_OUTPUT_DIR}/CSharp.ConnectedThresholdOutput2.png"
      210 250
      81 112
  IMAGE_MD5_COMPARE
    "${SimpleITK_TEST_OUTPUT_DIR}/CSharp.ConnectedThresholdOutput2.png"
    "a9695df04cde5b9c5481d8c253a5b3aa"
  )
