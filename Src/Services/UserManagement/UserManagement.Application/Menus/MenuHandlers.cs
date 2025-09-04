using MediatR;

namespace UserManagement.Application.Menus;

public sealed class GetMenuByIdHandler : IRequestHandler<GetMenuById, MenuDto?>
{
    private readonly IMenuReader _reader;
    public GetMenuByIdHandler(IMenuReader reader) => _reader = reader;
    public Task<MenuDto?> Handle(GetMenuById request, CancellationToken ct) => _reader.ById(request.Id, ct);
}

public sealed class ListMenusHandler : IRequestHandler<ListMenus, IReadOnlyList<MenuDto>>
{
    private readonly IMenuReader _reader;
    public ListMenusHandler(IMenuReader reader) => _reader = reader;
    public Task<IReadOnlyList<MenuDto>> Handle(ListMenus request, CancellationToken ct)
        => _reader.List(request.Skip, request.Take, ct);
}

public sealed class CreateMenuHandler : IRequestHandler<CreateMenu, long>
{
    private readonly IMenusWriter _writer;
    public CreateMenuHandler(IMenusWriter writer) => _writer = writer;
    public Task<long> Handle(CreateMenu request, CancellationToken ct)
        => _writer.Add(request.Key, request.DisplayName, request.Route, request.ParentId, request.SortOrder, ct);
}

public sealed class UpdateMenuHandler : IRequestHandler<UpdateMenu>
{
    private readonly IMenusWriter _writer;
    public UpdateMenuHandler(IMenusWriter writer) => _writer = writer;
    public async Task Handle(UpdateMenu request, CancellationToken ct)
        => await _writer.Update(request.Id, request.Key, request.DisplayName, request.Route, request.ParentId, request.SortOrder, ct);
}

public sealed class DeleteMenuHandler : IRequestHandler<DeleteMenu>
{
    private readonly IMenusWriter _writer;
    public DeleteMenuHandler(IMenusWriter writer) => _writer = writer;
    public async Task Handle(DeleteMenu request, CancellationToken ct)
        => await _writer.Delete(request.Id, ct);
}
