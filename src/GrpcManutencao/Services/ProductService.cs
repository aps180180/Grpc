using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcManutencao.Data;
using GrpcManutencao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GrpcManutencao.Services
{
    public class ProductService : ProductIt.ProductItBase
    {

        private readonly GrpcDbContext _grpcDbContext;
        public ProductService(GrpcDbContext grpcDbContext)
        {
            _grpcDbContext = grpcDbContext;
        }

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request,
         ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Nome) || string.IsNullOrEmpty(request.Descricao))
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Os dados não foram informados corretamente.")
                );
            }

            var product = new Product()
            {
                Nome = request.Nome,
                Descricao = request.Descricao

            };
            await _grpcDbContext.AddAsync(product);
            await _grpcDbContext.SaveChangesAsync();

            return await Task.FromResult(
                new CreateProductResponse
                {
                    Id = product.Id
                }
            );
        }

        public override async Task<ReadProductResponse> ReadProduct(ReadProductRequest request,
        ServerCallContext context)
        {
            if (request.Id <= 0)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Id do Produto é inválido")
                );
            }
            var product = await _grpcDbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (product is not null)
            {
                return await Task.FromResult(new ReadProductResponse
                {
                    Id = product.Id,
                    Nome = product.Nome,
                    Descricao = product.Descricao,
                    Status = product.Status

                });
            }
            throw new RpcException(
                    new Status(StatusCode.NotFound, $"Id do Produto {request.Id} não foi encontrado.")
                );

        }

        public override async Task<GetAllResponse> ListProduct(GetAllRequest request,
        ServerCallContext context)
        {
            var response = new GetAllResponse();
            var products = await _grpcDbContext.Products.ToListAsync();

            foreach (var product in products)
            {
                response.Product.Add(
                    new ReadProductResponse
                    {
                        Id = product.Id,
                        Nome = product.Nome,
                        Descricao = product.Descricao,
                        Status = product.Status
                    }
                );
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest request,
        ServerCallContext context)
        {
            if (request.Id <= 0 ||
                string.IsNullOrEmpty(request.Nome) ||
                string.IsNullOrEmpty(request.Descricao))
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Dados da requisição estão inválidos")
                );
            }
            var product = await _grpcDbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (product == null)
            {
                throw new RpcException(
                    new Status(StatusCode.NotFound, $"Produto id: {request.Id} não foi encontrado")
                );
            }
            product.Nome = request.Nome;
            product.Descricao = request.Descricao;
            product.Status = request.Status;

            _grpcDbContext.Products.Update(product);
            await _grpcDbContext.SaveChangesAsync();

            var response = new UpdateProductResponse()
            {
                Id = product.Id
            };
            return await Task.FromResult(response);

        }

        public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request,
        ServerCallContext context)
        {
            if (request.Id <= 0)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "O Id do Produto informado é invalido")
                );
            }
            var product = await _grpcDbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (product == null)
            {
                throw new RpcException(
                    new Status(StatusCode.NotFound, $"O Id do Produto {request.Id}  não foi encontrado")
                );
            }
            _grpcDbContext.Products.Remove(product);
            await _grpcDbContext.SaveChangesAsync();
            
            var response = new DeleteProductResponse()
            {
                Id = product.Id
            };
            return await Task.FromResult(response);


        }
    }

}