﻿using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CurrencyExchange.Cachgin
{
    public class CryptoCoinServiceWithCaching : ICryptoCoinRepository
    {
        private const string CacheProductKey = "coinsKeys";
        private readonly IMemoryCache  _memoryCache;
        private readonly ICryptoCoinRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CryptoCoinServiceWithCaching(IMemoryCache memoryCache, ICryptoCoinRepository repository, IUnitOfWork unitOfWork)
        {
            _memoryCache = memoryCache;
            _repository = repository;
            unitOfWork = _unitOfWork;

            if(!_memoryCache.TryGetValue(CacheProductKey, out _))
            {
                _memoryCache.Set(CacheProductKey, _repository.GetAll().ToList());
            }
        }

        public async Task<CryptoCoin> AddAsync(CryptoCoin entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            return entity;

        }

        public async Task<IEnumerable<CryptoCoin>> AddRangeAsync(IEnumerable<CryptoCoin> entities)
        {
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            return entities;
        }

        public Task<bool> AnyAsync(Expression<Func<CryptoCoin, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CryptoCoin>> GetAllAsync()
        {
            return Task.FromResult(_memoryCache.Get<IEnumerable<CryptoCoin>>(CacheProductKey));
        }

        public Task<CryptoCoin> GetByIdAsync(int id)
        {
            return Task.FromResult(_memoryCache.Get<List<CryptoCoin>>(CacheProductKey).FirstOrDefault(x => x.Id == id));
        }

        public async Task RemoveAsync(CryptoCoin entity)
        {
            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<CryptoCoin> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task UpdateAsync(CryptoCoin entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public IQueryable<CryptoCoin> Where(Expression<Func<CryptoCoin, bool>> expression)
        {
            return _memoryCache.Get<List<CryptoCoin>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
        }
        public async Task CacheAllProductsAsync()
        {
            _memoryCache.Set(CacheProductKey, await _repository.GetAll().ToListAsync());
        }

        public Task<CustomResponseDto<NoContentDto>> CryptoCoin()
        {
            throw new NotImplementedException();
        }

        public IQueryable<CryptoCoin> GetAll()
        {
            throw new NotImplementedException();
        }

        Task IGenericRepository<CryptoCoin>.AddAsync(CryptoCoin entity)
        {
            throw new NotImplementedException();
        }

        Task IGenericRepository<CryptoCoin>.AddRangeAsync(IEnumerable<CryptoCoin> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(CryptoCoin entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(CryptoCoin entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<CryptoCoin> entities)
        {
            throw new NotImplementedException();
        }
    }
}