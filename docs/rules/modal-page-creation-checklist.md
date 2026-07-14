# Modal 页面创建注意事项

在 ABP Framework 项目中创建模态框（Modal）Razor Page 时，必须注意以下规则：

## 1. 必须设置 `Layout = null`

模态框 `.cshtml` 文件必须在文件顶部设置 `Layout = null`，否则模态框内容会被包裹在 ABP 主题的完整布局（header、sidebar 等）中：

```csharp
@page
@model AppManager.Web.Pages.XXX.CreateModalModel
@{
    Layout = null;
}
```

## 2. 必须校验 `ModelState.IsValid`

模态框后端 `OnPostAsync()` 方法必须在调用业务服务前检查 `ModelState.IsValid`，防止表单校验不通过时仍然向后端发起请求：

```csharp
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    await _service.CreateAsync(Input);
    return NoContent();
}
```

**Why:** 即使前端有 jQuery unobtrusive validation，表单仍可能在模型绑定后校验失败（如空值、格式错误等）。来自日志证据：`ModelState is "Invalid"` 状态下 POST 请求仍到达后端并调用服务层，导致 500 错误而非正确的 400 验证响应。

**How to apply:** 在每个 `*Modal.cshtml.cs` 的 `OnPostAsync` 方法最开始添加 `if (!ModelState.IsValid) return BadRequest(ModelState);`。参考 `IisInstances/CreateModal.cshtml.cs:24-29`。

## 3. 前端使用 `abp.ModalManager` 打开

在 `Index.js` 中通过 ABP 的 `abp.ModalManager` 打开模态框，不要做路由跳转：

```javascript
$('#CreateButton').on('click', function () {
    var modal = new abp.ModalManager({
        viewUrl: abp.appPath + 'XXX/CreateModal'
    });
    modal.onResult(function () {
        dataTable.ajax.reload();
    });
    modal.open();
});
```

## 相关文件

- `src/AppManager.Web/Pages/IisInstances/CreateModal.cshtml.cs` — 正确实现示例