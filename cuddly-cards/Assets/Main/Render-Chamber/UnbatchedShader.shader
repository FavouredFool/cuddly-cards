Shader "KriptoFX/KWS/WaterTesselated" {
    Properties
    {
        srpBatcherFix("srpBatcherFix", Float) = 0
    }

    SubShader{

        Tags{ "RenderPipeline" = "HDRenderPipeline" "RenderType" = "HDLitShader" "Queue" = "Transparent-1" }

        Pass
            {
            ZWrite On


                Cull Back
                HLSLPROGRAM



float srpBatcherFix;
            //some code

            ENDHLSL
            }
    }
}