# CD_Scan Agent 规约

本文档供 AI Agent 在本仓库中修改代码时参考。用户文档见 [README.md](README.md)。

## 项目上下文

- **领域**：光通信 Interleaver（交织器）终测 CD（色散）工序的数据采集与导表
- **形态**：遗留 WinForms 单体应用，业务逻辑高度集中在 `Form1.cs`（约 2200 行）
- **目标**：保持现有行为兼容，优先小范围、可验证的补丁，避免大规模重构

## 目录与职责

```
CD_Scan/
├── CD_Scan/                    # 主项目源码
│   ├── Form1.cs                # UI + 流程编排（扫描 / 导表 / 预匹配）
│   ├── Form1.Designer.cs       # 窗体设计器（按钮、布局）
│   ├── Calc.cs                 # 命名空间 InterleaverDateKit：数据模型 + ILCalc
│   ├── LineFit.cs              # 命名空间 LinearFit：拟合算法
│   ├── Program.cs              # 入口
│   ├── Temp_config.txt         # 开发用配置样例
│   └── CD_IP.txt               # 仪器 IP 样例
├── *.csv, *_script_*.txt       # 测试数据样例（非源码，勿随意修改）
├── CD_Scan.sln
├── README.md                   # 用户文档
└── AGENTS.md                   # 本文件
```

**不要修改**：`bin/`、`obj/`、`.vs/`、无关的样例 CSV/脚本。

## 关键调用链

修改功能前，先定位对应链路：

### 导表

```
outputresult_Click / button5_Click / anaylsedata
  → calcResult()
    → Data.getvaluefromfile()     # 读原始 CSV
    → 解析 *_script_*.txt
    → ILCalc.Calc*()              # 各指标计算
  → 写 *-导表.csv（标题、单位、MAX-Value、逐信道）
```

### 仪器扫描

```
button2_Click → Connect_Click()           # pdlaClient 连接
button1_Click → initpara() + Normalize()
CDScan_Click  → initpara() + Measure()
TriggerProgessEvent → getResultValue() → savedata()   # 写原始 CSV
```

### 预匹配

```
button6_Click
  → readinifile()        # parameter.ini
  → readtestfile()       # amalgamate.ini，校验 COER/DCM 文件存在
  → sortcoer()           # 按 CD 偏差排序 COER
  → fileamalgamate()     # 多文件光谱叠加
  → anaylsedata()        # 导表 + parameter.ini 阈值判定
```

### 全局数据缓冲

`InterleaverDateKit.Data` 使用静态 `ArrayList` 字段（`m_aldblFreq1`、`m_aldblIL`、`m_aldblCD` 等）。`fileamalgamate` 前会调用 `Data.zerodata()`。

## 命名空间

| 命名空间 | 文件 | 用途 |
|----------|------|------|
| `CD_Scan` | Form1.cs, Program.cs | UI 与主流程 |
| `InterleaverDateKit` | Calc.cs | 数据、INI、ILCalc |
| `LinearFit` | LineFit.cs | 最小二乘 / 两点直线拟合 |

保持现有划分，不要将 Calc 逻辑迁入 Form1 或反之。

## 修改约束

### 必须遵守

1. **最小改动**：不重构 `Form1.cs` 大段逻辑，不引入新框架或 NuGet 包，除非用户明确要求
2. **编码**：读写配置使用 GBK（`Encoding.GetEncoding("GBK")` / `gb2312`），与现有文件一致
3. **文件命名约定**：
   - 原始：`{前缀}-ODD.csv` / `{前缀}-EVEN.csv`
   - 导表：`{前缀}-ODD-导表.csv`
   - 脚本：`{outputfile}_script_{ODD|EVEN}_{PDL|NOPDL}.txt`
4. **硬编码**：`outputresult_Click` 中 `m_nCalGhz = 184000` 会覆盖 `{PN}.ini` 的 `CalGhz` 读取；改频率逻辑时需同步 `calcResult`、`ResbyIL.m_nChFirst` 及脚本第 2 行
5. **信道行数**：`nRealLine` 受 `nchspace` 影响（`<100`、 `>100`、 `>150` 三分支），改输出范围时三处导表写文件逻辑需一并检查（`outputresult_Click`、`button5_Click`、`anaylsedata` 有重复代码）
6. **备份策略**：目标 CSV 已存在时备份为 `*_N.csv`，不弹窗覆盖（相关 MessageBox 已注释）

### 仪器与外部 DLL

- 依赖 `Ag86038x_Engine.dll`、`InstrumentObjects.dll`、`RemoteClient.dll`
- 无 DLL 或无实机时，Agent **无法验证扫描/归零**；仅能验证编译与导表逻辑
- 不要删除或替换这些引用，除非用户明确要求换仪器方案

### 不要提交

- 本地绝对路径（`Temp_config.txt` 中的开发路径）
- 生产环境 `CD_IP.txt` 中的真实 IP
- `bin/`、`obj/` 构建产物

## 常见任务指引

| 用户意图 | 优先查看 | 注意点 |
|----------|----------|--------|
| 新增导表指标 | `Form1.calcResult` 的 `switch` + `ILCalc` 新方法 | 更新 README 脚本格式；样例 CSV 回归 |
| 调整信道范围/输出行数 | 脚本第 2 行 + `nRealLine` / `nRealStartIndex` | `nchspace` 三分支；三处写 CSV 逻辑可能重复 |
| 修改合格判定 | `anaylsedata` + `parameter.ini` | `paravalue_{N}` 最后一项为 IL Ripple；`position_*` 对应导表列索引 |
| 换仪器 / 连接问题 | `pdlaClient`、`initpara`、`getResultValue` | 外部 DLL API，需实机 |
| 修复导表数值 | `ILCalc.Calc*` + `LineFit` | 用仓库样例 `*.csv` + `*_script_*.txt` 对比 `*-导表.csv` |
| 预匹配逻辑 | `readtestfile`、`sortcoer`、`fileamalgamate` | `amalgamate.ini` 节 `[COER]`、`[DCM1]`、`[DCM2]` |
| UI 文案 / 按钮 | `Form1.Designer.cs` + 对应 `*_Click` | Designer 与事件处理分离 |

## 配置与脚本契约（速查）

### parameter.ini（节 `8048M_1` / `8048M_2`）

- `Paraname`：逗号分隔参数名
- `Parasum`：参数个数
- `CheckCD`：COER 排序用参考 CD
- `position_{n}`：导表结果列索引
- `paravalue_{n}`：阈值；最后一项为 IL Ripple 上限

### amalgamate.ini

- `[COER]`：`coer_sum`、`SN_{n}`、`PATH_{n}`、`script_odd_path`、`script_even_path`、`DCM_use`
- `[DCM1]` / `[DCM2]`：`dcm1_sum` / `dcm2_sum`、`SN_{n}`、`PATH_{n}`
- 期望文件：`{PATH}\{SN}=ODD.csv`、`{SN}=EVEN.csv` 及对应 `-导表.csv`

### 脚本行格式（第 3 行起）

```
指标名,参考(ITU或CF(pb:dep)),通带宽度,深度/类型,...,拟合阶数,GD类型
```

`calcResult` 中按 `strArray[0]` 分支；新增指标须与此 CSV 列格式一致。

## 构建与验证

### 构建

```powershell
msbuild CD_Scan.sln /p:Configuration=Debug
```

需要 .NET Framework 4.6.1+ 与 MSBuild（Visual Studio 或 Build Tools）。

### 验证

- **无单元测试**
- 导表逻辑：用根目录样例 `*=终测CD测试=*-ODD.csv` + `*_script_ODD_NOPDL.txt`，通过 UI「独立导表」或分析 `calcResult` 输出
- 声明完成前：至少确认 **C# 编译通过**；若改计算逻辑，说明使用的样例文件及预期差异

## 文档同步

以下变更须同步更新 [README.md](README.md)：

- 脚本格式、配置项、按钮行为
- 文件命名约定
- 新增指标类型

新增 Agent 约定（禁止修改的模块、特殊回归要求等）写入本文件。
