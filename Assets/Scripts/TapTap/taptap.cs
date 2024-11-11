using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TapSDK.Core;
using TapSDK.Compliance;
using TapSDK.Login;

public class taptap : MonoBehaviour
{
    private const string clientId = "q9f6zyfuj8fac1jk9v";
    private const string clientToken = "HZSBvc45Zqzp4RVMwGbALLLKBGmTdvnH5ZCyWSq8";
    private void Awake()
    {
        // 核心配置
        TapTapSdkOptions coreOptions = new TapTapSdkOptions
        {
            // 客户端 ID，开发者后台获取
            clientId = clientId,
            // 客户端令牌，开发者后台获取
            clientToken = clientToken,
            // 地区，CN 为国内，Overseas 为海外
            region = TapTapRegionType.CN,
            // 语言，默认为 Auto，默认情况下，国内为 zh_Hans，海外为 en
            preferredLanguage = TapTapLanguageType.zh_Hans,
            // 是否开启日志，Release 版本请设置为 false
            enableLog = true
        };
    
        // 合规认证配置
        TapTapComplianceOption complianceOption = new TapTapComplianceOption
        {
            showSwitchAccount = false,
            useAgeRange = false
        };
        // 其他模块配置项
        TapTapSdkBaseOptions[] otherOptions = new TapTapSdkBaseOptions[]
        {
            complianceOption
        };
        // TapSDK 初始化
        TapTapSDK.Init(coreOptions, otherOptions);
    }
    
    async void Start()
    {
        try
        {
            // 定义授权范围
            List<string> scopes = new List<string>
            {
                TapTapLogin.TAP_LOGIN_SCOPE_BASIC_INFO
            };
            // 发起 Tap 登录
            var userInfo = await TapTapLogin.Instance.LoginWithScopes(scopes.ToArray());
            Debug.Log($"登录成功，当前用户 ID：{userInfo.uniontId}");
        }
        catch
        {
            Application.Quit();
        }
        
    }
    
}
