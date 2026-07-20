#!/usr/bin/env python3
"""更新所有 .csproj 项目文件的 TargetFramework 与 Volo.Abp 包版本。"""

import glob
import re
import os

# 目标值
TARGET_FRAMEWORK = "net10.0"
ABP_VERSION = "10.5.0"

# 搜索所有 .csproj 文件
csproj_files = glob.glob("**/*.csproj", recursive=True)

if not csproj_files:
    print("❌ 未找到任何 .csproj 文件")
    exit(1)

updated_count = 0

for filepath in sorted(csproj_files):
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()

    original = content

    # 1. 替换 TargetFramework（例如 net8.0 → net10.0）
    content = re.sub(
        r"<TargetFramework>net[\d.]+</TargetFramework>",
        f"<TargetFramework>{TARGET_FRAMEWORK}</TargetFramework>",
        content,
    )

    # 2. 替换所有 Volo.Abp.* 包引用的版本号
    content = re.sub(
        r'(<PackageReference\s+Include="Volo\.Abp\.[^"]*"\s+Version=")[^"]*(")',
        rf"\g<1>{ABP_VERSION}\g<2>",
        content,
    )

    if content != original:
        with open(filepath, "w", encoding="utf-8") as f:
            f.write(content)
        print(f"✅ 已更新: {filepath}")
        updated_count += 1
    else:
        print(f"⏭️ 跳过 (无变化): {filepath}")

print(f"\n📊 总计: {len(csproj_files)} 个项目文件, {updated_count} 个已更新")